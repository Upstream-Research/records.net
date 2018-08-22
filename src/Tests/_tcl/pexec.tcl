# like the exec command: execute a process
# unlike the exec command: the process exit code is returned and without generating a TCL error
proc pexec args {
    set results ""
    set errorcode 0
    try {
        set results [eval exec $args]
    } trap CHILDSTATUS {results options} {
        set errorcode [lindex [dict get $options -errorcode] 2]
    }
    return $errorcode
}
