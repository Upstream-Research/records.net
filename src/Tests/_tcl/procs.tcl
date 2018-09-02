##  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  ##
##  Subject to the MIT License. See LICENSE file in top-level directory. ##

## Helper functions for executing console-based tests via TCL

# read each line of a file into a list
proc read_string_list_from_file {file_name} {
    set str_list [list]
    
    if {[file exists $file_name]} {
        set fchan [open $file_name RDONLY]
        try {
            # if the last line is empty, then we need to ignore it
            gets $fchan str_item
            while {1 != [eof $fchan]} {
                lappend str_list $str_item
                gets $fchan str_item
            }
            if {0 < [string length $str_item]} {
                lappend str_list $str_item
            }
        } finally {
            close $fchan
            unset fchan
        }
    }
    
    return $str_list
}


# Read command arguments encoded in a text file:
#  each separate line is a separate argument.
#  lines that begin with '#' are ignored (they are treated as comments)
proc read_args_from_file {file_name} {
    set str_list [read_string_list_from_file $file_name]
    lappend argv
    foreach str_item $str_list {
        if {"#" == [string range [string trimleft $str_item] 0 0]} {
            # comment, skip this item
        } else {
            lappend argv $str_item
        }
    }
    
    return $argv
}


# TEST SELECTION
# Navigate into subdirectories and build a list of test cases
# 01 note current directory
# 02 move to up dir hierarchy and push dir names onto a stack until there is no _test.dirs.txt file; this is the root test_in directory.
# 03 pop the whole dir stack into a list, this is the relative path to the current test directory
# 04 for each ancestor directory (starting from the root),
#      write a line with the list of previous ancestors, and the ancestor directory name itself
# 05 read _test.cases.txt into a list to get the test cases defined in this directory
# 06 for each test case
#     write a line with the list of ancestor directory names, the containing directory name, and the test case name
# 07 read _test.dirs.txt into a list to get the subdirectories containing tests
# 08 for each sub-directory
#     write a line with the list of ancestor directory names, and the subdirectory name itself
#     recurse to step 05


# returns a list/tuple with the following elements:
#   {root_dir test_dir test_dir_name_list}
# test_dir_name_list is the same as [file split test_dir]
proc get_test_case_dirs {in_dir test_dir_file_name} {

    # NOTE: "directory name" means here the unqualified name of a directory, like "lib" (not "/usr/lib")
    # TCL imitates the unix dirname command, which means "parent directory path"; not exactly name of directory.
    set start_dir [pwd]
    if {0 < [string length $in_dir]} {
        set start_dir [file join $start_dir $in_dir]
    }
    
    set start_dir_name [file tail $start_dir]
    set parent_dir_name_list [list]
    set parent_dir_name [file tail $start_dir]
    set parent_dir [file dirname $start_dir]
    set root_dir $start_dir
    while {[string length $parent_dir] 
        && [file exists [file join $parent_dir $test_dir_file_name]]
    } {
        set root_dir $parent_dir
        lappend parent_dir_name_list $parent_dir_name
        set parent_dir_name [file tail $parent_dir]
        set parent_dir [file dirname $parent_dir]
    }    
    # reverse the parent dir_name_list so that the greatest ancestor comes first (instead of last)
    set test_dir_name_list [lreverse $parent_dir_name_list]
    set test_dir ""
    if {0 < [llength $test_dir_name_list]} {
        set test_dir [eval [concat file join $test_dir_name_list]]
    }
    
    return [list $root_dir $test_dir $test_dir_name_list]
}

# recursive sub-procedure to write out testcases in subdirectories
proc lappend_child_test_case_records {
    test_case_record_list_var_name
    test_subdir 
    root_dir
    test_dir_file_name 
    test_case_file_name 
    directory_test_case_name
} {
    upvar $test_case_record_list_var_name test_case_record_list

    # find the test cases in current directory
    set test_case_name_list [read_string_list_from_file [file join $root_dir $test_subdir $test_case_file_name]]
    foreach test_case_name $test_case_name_list {
        set field_list [list $test_subdir $test_case_name]
        lappend test_case_record_list $field_list
    }
    
    # find the test cases in each subdirectory
    set child_test_dir_name_list [read_string_list_from_file [file join $root_dir $test_subdir $test_dir_file_name]]
    set child_test_subdir_list {}
    foreach child_test_dir_name $child_test_dir_name_list {
        set child_test_subdir [file join $test_subdir $child_test_dir_name]
        lappend child_test_subdir_list $child_test_subdir
    }
    
    # recurse into each subdir
    foreach child_test_subdir $child_test_subdir_list {
        set field_list [list $child_test_subdir $directory_test_case_name]
        lappend test_case_record_list $field_list
        lappend_child_test_case_records test_case_record_list $child_test_subdir $root_dir $test_dir_file_name $test_case_file_name $directory_test_case_name
    }
    
    return
}

# recurse through a directory hierarchy and build a list of test cases defined within it.
proc get_test_case_record_list {
    root_dir
    test_dir_name_list
    test_dir_file_name
    test_case_file_name
    directory_test_case_name
} { 
    # get the relative path to this directory from the test case root
    set test_case_record_list [list]
    set test_subdir ""
    foreach test_dir_name $test_dir_name_list {
        set test_subdir [file join $test_subdir $test_dir_name]
        set field_list [list $test_subdir $directory_test_case_name]
        lappend test_case_record_list $field_list
    }
    
    
    lappend_child_test_case_records test_case_record_list $test_subdir $root_dir $test_dir_file_name $test_case_file_name $directory_test_case_name
        
    return $test_case_record_list
}


# given a list of test case records (from proc get_test_case_record_list),
#  execute each test described in each record
proc invoke_test_case_record_list {
    test_case_record_list
    in_root_dir
    out_root_dir
    directory_test_case_name
    directory_test_exec_file_name
} {
        
    set test_exec_fn_dict [dict create]
    
    # ensure output root directory exists
    if {![file exists $out_root_dir]} {
        puts stderr "mkdir $out_root_dir"
        file mkdir $out_root_dir
    }
    
    foreach test_case_field_list $test_case_record_list {
        lassign $test_case_field_list test_case_subdir test_case_name
        set in_dir [file join $in_root_dir $test_case_subdir]
        set out_dir [file join $out_root_dir $test_case_subdir]
        if {$directory_test_case_name == $test_case_name} {
            # if this is a directory (i.e. a "parent" test case),
            #   then we must "source" a script that knows how to execute the child test cases
            #   and we must ensure the output directory exists
            puts stderr "mkdir $out_dir"
            file mkdir $out_dir
            set test_exec_file_path [file join $in_dir $directory_test_exec_file_name]
            if {[file exists $test_exec_file_path]} {
                puts stderr "source $test_exec_file_path"
                set test_case_exec_fn [source $test_exec_file_path]
                dict set test_exec_fn_dict $test_case_subdir $test_case_exec_fn
            }
        } else {
            # if this is a test case name,
            #  then find the execution function assigned to this directory
            #  and execute it
            if {[dict exists $test_exec_fn_dict $test_case_subdir]} {
                set test_case_exec_fn [dict get $test_exec_fn_dict $test_case_subdir]
                puts stderr "execute_test $test_case_subdir $test_case_name"
                apply $test_case_exec_fn $test_case_name $in_dir $out_dir
            }
        }
    }
}


# helper proc to select and execute tests
proc run_test_cases args {    
    global env
    set out_root_dir $env(TC_OUT_ROOT)
    set exp_root_dir $env(TC_EXPECT_ROOT)
    set in_root_dir $env(TC_IN_ROOT)
    set test_dir_file_name $env(TEST_DIR_FILE_NAME)
    set test_case_file_name $env(TEST_CASE_FILE_NAME)    
    set directory_test_case_name $env(DIRECTORY_TEST_CASE_NAME)
    set directory_test_exec_file_name $env(DIRECTORY_TEST_EXEC_FILE_NAME)
    set DIFF $env(DIFF)
    
    set in_dir ""
    if {0 < [llength args]} {
        set in_dir [lindex $args 0]
    }

    lassign [get_test_case_dirs $in_dir $test_dir_file_name] \
        root_dir \
        test_dir \
        test_dir_name_list
    
    # if in_dir is not in the test input directory,
    #   then we should run all tests from there
    if {0 == [llength $test_dir_name_list]} {
        set in_dir $in_root_dir
        lassign [get_test_case_dirs $in_dir $test_dir_file_name] \
            root_dir \
            test_dir \
            test_dir_name_list
    }
    
    set test_case_record_list [get_test_case_record_list $root_dir $test_dir_name_list $test_dir_file_name $test_case_file_name $directory_test_case_name]
    invoke_test_case_record_list $test_case_record_list $in_root_dir $out_root_dir $directory_test_case_name $directory_test_exec_file_name

    set out_dir [file join $out_root_dir $test_dir]
    set exp_dir [file join $exp_root_dir $test_dir]
    
    # TODO don't assume the diff program is available,
    #  and do something a bit more useful than this;
    #  use the test_case_record_list to find expected and actual output files, compare them,
    #    and output a record that gives the pass/fail result of each test.
    puts stderr "$DIFF -r $exp_dir $out_dir"
    set diff_exit_code [pexec $DIFF -r $exp_dir $out_dir]
    if {$diff_exit_code} {
        # this is lame, we should show the output of the diff command;
        puts stdout "SOME TESTS FAILED; try running diff again to find which outputs are unexpected."
    } else {
        puts stdout "ALL TESTS PASSED"
    }
}

