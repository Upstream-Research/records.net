##  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  ##
##  Subject to the MIT License. See LICENSE file in top-level directory. ##

proc test_csv_record_translate { 
    test_name
    test_out_file_path
    test_argv
} {
    global env
    set TCON $env(TCON)

    set test_cmd [concat exec $TCON "rcd-csv-translate" -o $test_out_file_path $test_argv]

    puts stderr $test_cmd
    eval $test_cmd
}


# final statement in this file is an anonymous function,
#  when this file is "sourced", this function will be the return value,
#  and it will be used to execute each test in this directory
return { {test_name in_dir out_dir} 
{
    # pass, no test cases at this level
}
}
