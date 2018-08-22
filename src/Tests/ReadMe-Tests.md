Automated Testing Methods
-------------------------

An I/O-centric unit testing mechanism.

Revised: 2018-08-21 (db)


## Run the Tests

You need a TCL interpreter.
And you may need some unix tools like `diff`.  (Consider GnuWin32 if you are using Windows).

    > tclsh run_tests.tcl

Each test case writes an output file to the `test_out` directory.
You can check test results by comparing the `test_out` to the `test_expect` directory using a diff program.

To run tests selectively, open the TCL interpreter,
run (i.e. "source") the `_test.env.tcl` script,
and execute the `run_test_cases` proc with an appropriate subdirectory name:

    > tclsh
    % source _test.env.tcl
    % run_test_cases test_in/field_schema_spec


## What Is Going On Here?

This test suite demonstrates an input/output-centric method for unit testing,
which might be considered an alternative to xUnit-style testing
(although it is better to see it as a complement).
This method of testing is unconventional,
and so I'm using this project as an excuse to try and demonstrate its strengths
as well as to simply explore its potential.
This means that you are looking at a research project,
and I hope you are not too terribly upset by the lack of polish and the loose ends.

To be sure, we cannot really test the units directly using this method (yes this is a drawback),
but we can reduce much of the important testing to a process of preparing inputs and expected outputs
without having to write and maintain a bunch of tedious, highly specialized, "assertion" code
(as one finds in "xUnit" unit testing).
The test developer must still write some code to invoke the unit,
but this is done as a thin "integration" exercise
by writing a command-line program that controls the unit via standard I/O.
Often, writing such a "unit controller" is quite easy to do, 
it helps refine the unit's interface,
and it yields a useful tool as a bonus.
The test cases themselves are often very easy to write;
indeed, this is one of the main goals:
there should be very few obstacles in the way of writing new test cases.

Running a whole group (aka. "suite") of tests requires some automation of many unit controllers
and for this process I have chosen to use TCL,
under the assumption that TCL is cross-platform, small, and widely available.
I am not a great fan of TCL, 
but as the "Tool Command Language",
I could hardly be surprised that it was well suited to this job.
Other options such as Bash, Make, Powershell, Python, or even a Lisp-based interpreter
seemed to violate one or more of the criteria previously mentioned.

I've tried to build-up the testing infrastructure in layers.
The first layer is the set of unit controllers which are directly linked to the units.
On top of this is a layer of test case definitions,
which consists of a directory hierarchy full of test case attribute and input files
and which are independent from TCL.
The test case definitions are interpreted by the automated test infrastructure,
whose job it is to read the test definitions, select the test cases,
invoke them, and to evaluate the test case results.

To make things easy, I've tried to limit the TCL code to the job of
selection and invocation of the test cases,
the test result evaluation is done using any recursive `diff` tool
(although here I've automated this too simply by invoking the basic `diff` command from TCL).
Down the road, I'd like to write a separate test case selection tool that does not need TCL.
This would leave TCL only with the job of test case invocation,
which for a .NET-based system, might best be replaced with PowerShell,
or some other scriptable .NET program.

## Test Case Definitions

Tests are defined in the `test_in` and the `test_expect` directories.
Output from invoking a test case is to be stored in the `test_out` directory.
All three directories should have the same directory structure.
The `test_in` directory contains the test case names, attributes, and invocation procedures.
Invoking a test case will produce one or more output files.
The `test_expect` directory should contain copies of files containing the expected output for each test case.
Test case names and attributes are defined in each directory in the hiearchy,
in a file named `_test.cases.txt`.
Presently, this is a file of test case names only,
but in a more sophisticated test case suite, 
these files would contain additional meta-data "attributes",
which would further aid in test case selection and invocation.
Each subdirectory may itself be considered as a "parent" test case.
The list of such subdirectories is stored in files named `_test.dirs.txt`
Invoking test cases involves executing a TCL procedure
which knows how to read test case inputs, invoke the unit controller, and save the output.
All tests within the same subdirectory will be executed by a TCL procedure
defined in the file named `_test.exec.tcl` within that subdirectory.

Thus the test selection unit's job is to recurse down the directory hierarchy under the `test_in` directory,
read the `_test.cases.txt` and the `_test.dirs.txt` files,
and emit a list of test "parent" directory names and test cases to be invoked.

The test invocation unit's job is to go through the list of test cases
and for each parent test case (subdirectory),
load the `_test.exec.tcl` script to get the parent test case procedure,
and for each child test case,
invoke it by passing its name to the parent test case procedure.




