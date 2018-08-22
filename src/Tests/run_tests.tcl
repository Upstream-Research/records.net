source _test.env.tcl

# delete contents of test output directory before running all tests
if {[file exists $env(TC_OUT_ROOT)]} {
    puts stderr "rmdir -r $env(TC_OUT_ROOT)"
    file delete -force $env(TC_OUT_ROOT)
}

run_test_cases $env(TC_IN_ROOT)
