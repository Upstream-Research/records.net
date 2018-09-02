##  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  ##
##  Subject to the MIT License. See LICENSE file in top-level directory. ##


# execute one test for the field_schema_spec
proc test_field_spec {test_name tc_out_file_path tc_argv} {
    # Execute a field_schema_spec subcommand from our test console,
    #  using a set of arguments specific to the test case
    #  and pipe stdout to a file so we can compare it to an expected output later
    global env
    set TCON $env(TCON)
    set test_cmd [concat exec $TCON field_schema_spec-print $tc_argv > $tc_out_file_path]
    puts stderr $test_cmd
    eval $test_cmd
}


# final statement in this file is an anonymous function,
#  when this file is "sourced", this function will be the return value,
#  and it will be used to execute each test in this directory
return { {test_name in_dir out_dir} 
{
    set tc_out_file_name $test_name
    append tc_out_file_name ".txt"
    set tc_out_file_path [file join $out_dir $tc_out_file_name]

    set tc_arg_file_name $test_name
    append tc_arg_file_name ".args.txt"
    set tc_arg_file_path [file join $in_dir $tc_arg_file_name]

    set tc_argv [read_args_from_file $tc_arg_file_path]
    test_field_spec $test_name $tc_out_file_path $tc_argv
}
}
