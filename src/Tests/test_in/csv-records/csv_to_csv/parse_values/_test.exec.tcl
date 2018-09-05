
#extern proc test_csv_record_translate

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

    # the -x option will cause field values to be parsed according to their type
    set test_argv [list -x $test_in_file_path]

    test_csv_record_translate $test_name $test_out_file_path $test_argv
}
}
