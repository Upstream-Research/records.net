
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
    
    # parse locale from test name
    # (20180905 (db) This would be a great place to use "attributes" from the _test.cases.txt file,
    #  but presently, our test framework does not support this feature,
    #  so we parse test attributes from the test name.)
    # file name must of the form "{base_name}+{locale_in}+to+{locale_out}.csv"
    # thus: [0]=base_name, [1]=locale_in, [2]="to", [3]=locale_out
    # the special name 'default' must be detected and handled accordingly
    set default_locale_name "default"
    set test_name_part_separator "+"
    set test_name_part_list [split $test_name $test_name_part_separator]
    set locale_in [lindex $test_name_part_list 1]
    set locale_out [lindex $test_name_part_list 3]
    
    set test_argv [list]
    if {$locale_in != $default_locale_name} {
        set test_argv [concat $test_argv [list -L $locale_in]]
    }
    if {$locale_out != $default_locale_name} {
        set test_argv [concat $test_argv [list -l $locale_out]]
    }
    
    # the -x option will cause field values to be parsed according to their type
    # we need this option so that numbers will get parsed according to locale conventions.
    set test_argv [concat $test_argv [list -x $test_in_file_path]]

    test_csv_record_translate $test_name $test_out_file_path $test_argv
}
}
