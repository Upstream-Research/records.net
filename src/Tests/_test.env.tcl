##  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  ##
##  Subject to the MIT License. See LICENSE file in top-level directory. ##

# set global variables into environment variable array
#   so that they can be inherited by child processes.
#   note that test case executables will inherit this environment,
#     but they generally shouldn't be written to depend on these special environment variables.

# root of the test directories
array set env [list TC_ROOT [pwd]]

# root of the test case output file directory
array set env [list TC_OUT_ROOT [file join $env(TC_ROOT) "test_out"]]

# root of the expected output file directory
array set env [list TC_EXPECT_ROOT [file join $env(TC_ROOT) "test_expect"]]

# root of the test case definition directory
array set env [list TC_IN_ROOT [file join $env(TC_ROOT) "test_in"]]

array set env {TEST_DIR_FILE_NAME "_test.dirs.txt"}
array set env {TEST_CASE_FILE_NAME "_test.cases.txt"}
array set env {DIRECTORY_TEST_EXEC_FILE_NAME "_test.exec.tcl"}
array set env {DIRECTORY_TEST_CASE_NAME "."}


array set env {DIFF diff}
#array set env {FC cmp}

array set env [list TCON [file join [file dirname $env(TC_ROOT)] "bin" "debug" "Upstream.System.TestConsole.exe"]]
array set env [list RCD_ED [file join [file dirname $env(TC_ROOT)] "bin" "debug" "rcd-editor.exe"]]


set test_util_dir [file join $env(TC_ROOT) "_tcl"]

source [file join $test_util_dir pexec.tcl]
source [file join $test_util_dir procs.tcl]

