##  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  ##
##  Subject to the MIT License. See LICENSE file in top-level directory. ##


source _test.env.tcl

# delete contents of test output directory before running all tests
if {[file exists $env(TC_OUT_ROOT)]} {
    puts stderr "rmdir -r $env(TC_OUT_ROOT)"
    file delete -force $env(TC_OUT_ROOT)
}

run_test_cases $env(TC_IN_ROOT)
