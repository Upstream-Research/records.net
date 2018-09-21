##  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  ##
##  Subject to the MIT License. See LICENSE file in top-level directory. ##

proc test_record_editor { 
    test_name
    test_out_file_path
    test_in_file_path
    test_script_file_path
    test_argv
} {
    global env
    set RCD_ED $env(RCD_ED)

    set test_cmd [concat exec $RCD_ED -q -o $test_out_file_path -J $test_script_file_path $test_in_file_path $test_argv]

    puts stderr $test_cmd
    eval $test_cmd
}


# final statement in this file is an anonymous function,
#  when this file is "sourced", this function will be the return value,
#  and it will be used to execute each test in this directory
return { {test_name in_dir out_dir} 
{
    set test_out_file_name $test_name
    append test_out_file_name ".csv"
    set test_out_file_path [file join $out_dir $test_out_file_name]

    set test_in_file_name $test_name
    append test_in_file_name ".csv"
    set test_in_file_path [file join $in_dir $test_in_file_name]

    set test_script_file_name $test_name
    append test_script_file_name ".ed.csv"
    set test_script_file_path [file join $in_dir $test_script_file_name]

    set test_arg_file_name $test_name
    append test_arg_file_name ".args.txt"
    set test_arg_file_path [file join $in_dir $test_arg_file_name]

    set test_argv [read_args_from_file $test_arg_file_path]
    test_record_editor $test_name $test_out_file_path $test_in_file_path $test_script_file_path $test_argv
}
}
