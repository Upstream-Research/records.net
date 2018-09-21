records.net
-----------

.NET Tabular Data and Data Record Library

*Copyright (c) 2018 Upstream Research, All Rights Reserved.*

The source code for this library is made available under an "MIT" license,
See the "LICENSE" file for details.

Revised: 2018-09-21 (db)

_(20180921 (db) Warning: this is "alpha" quality code.
It works, but it is not yet ready for use in a production environment.
It has been released in the interest of getting it out into the community,
where people can see it and perhaps take an interest in it.)_


## Overview

This is a data support library intended to provide generalized access to tabular/record data.
It is somewhat similar to the DataSets in ADO.NET (i.e. System.Data),
but with a different emphasis.

The goals are to provide a finer set of core interfaces than System.Data
as well as to provide a .NET Standard implementation for basic tabular data access.
It provides, for example, interfaces such as `IRecordAccessor` and `IRecordCollection`
which may be implemented by a variety of sources,
including CSV, database, JSON, XML, .NET reflection or other back-end sources.

This library provides an alternative and a complement to ORM (Object/Relational Mapping) frameworks.
It is an alternative because it does not try to hide the tabular nature of relational data,
yet it is complementary because one can build an ORM framework on this library.

One of the strengths intended for this library is to provide a level of abstraction for tabular data,
which allows for simpler transmission, transformation, and encoding of records
at the expense of strongly-typed "entity" objects 
(although strongly-typed records are still possible with this library).
Since the creation of an effective set of abstractions is a major goal of this project,
it is anticipated that the existing object interfaces will need to reworked several times
before they reach their final form.
Therefore this project, is very much a work-in-progress.

Some code documentation has been made available online for convenience,
however it will not always be 100% up-to-date:

[Records.Net API documentation](https://upstream-research.github.io/projects/records.net/helpdocs)


## Some Remarks On Existing Data Libraries and Frameworks

### ADO.NET ###
ADO.NET provides two different groups of objects: 

1. "DbData": Database I/O objects, such as `IDataReader` and `IDbConnection`
2. "DataSets": Data structure objects, such as `DataSet` and `DataTable`

These are mostly independent groups; perhaps they should have been placed in separate namespaces.

In .NET Core 1.0, Microsoft considered `System.Data` "unsupported"
and yet provided some implementation of the DbData objects.
It appears that `System.Data` will be supported in .NET Core 2.0 and .NET Standard 2.0.

### ORM/Entity Frameworks ###
Object/Relational Mapping (ORM) frameworks rely on on "Plain Old Objects", or "Entity" objects
which attempt to encode record fields as strongly-typed object properties.
This is often very convenient, but it comes at the expense of generality;
to handle such entity objects more generally as abstract records, 
complicated systems involving reflection and collection manipulation have been devised.


## The Core Record Objects

The core of this library lies in the `IRecordAccessor` interface.
This interface is intended to be relatively light-weight
and capable of capturing a reduced set of dictionary operations.
It is possible to implement an `IRecordAccessor` with a varieity of objects,
including `System.Data.DataRow`, `System.Data.IDataRecord`, `System.Collections.Generic.IDictionary`,
as well as by simple entity objects - either dynamically using reflection 
or statically using specialized code (generated or hand-written).

Record collections are exposed through an `IRecordCollection` interface,
which gives access to an implementation of `IEnumerator<IRecordAccessor>`
and which provides a way to navigate records within a "backend" collection.
This "navigation" is done by making a somewhat non-standard use of an `IEnumerator` interface:
when moving the enumerator, the object reference returned by the `Current` property is not assumed to change,
however, the state of the `Current` property changes and the data that it _accesses_ changes.
The end result is to retain one instance of the `IRecordAccessor` interface
rather than wrapping every back-end record object with an adapter object that implements `IRecordAccessor`.

The library provides a basic implementation of `IRecordCollection` called `ArrayRecordList`.
This class implements `IRecordCollection` using a dynamic list of static object array records;
it provides an alternative to `System.Data.DataTable` without the dependency on `System.Data`.


## Project Layout

There are several library assembly projects in this code base.
The libraries are organized by dependency.
The core library, `Records`, has no significant dependencies and is built to be compatible with .NET Core 1.0.
Derived libraries, like `Records.Data` have a dependency on `System.Data`.


## Testing

Presently, this project does not use a conventional unit testing framework.
The approach to testing taken here is to develop light-weight console programs
that attempt to reach a high level of "code coverage"
without the introduction of extra dependencies.
Testing the code involves invoking the test console program(s) repeatedly with different arguments.
Validating the code's behavior involves comparing the output of the programs with an expected output.
A very simple test harness has been implemented in TCL 
that uses a file comparison _DIFF_ tool to compare expected to actual program outputs.
This is, admittedly unconventional and bound to evoke disagreement,
but the author believes there are several benefits to this approach that justify giving it a try.
That said, the test code at the time of writing this isn't very good, 
but improvements aren't difficult to imagine.
There has been little time for finishing the library code, much less testing it.

The test console programs also serve as examples for how to use the APIs.
Presently, there is a "master" console program called `TestConsole` which wraps
the other console programs and calls them as subcommands.
These programs demonstrate some simple CSV reading and writing,
a parser for a simple CSV-compatible record schema specification mini-language,
and an interactive and scriptable CSV record editor.

The record editor is also compiled into a stand-alone executable called `rcd-editor`,
and it is by far the most complicated (and arguably the most useful) of the test programs.
It provides an interactive, scriptable command interpreter for manipulating records
in a record collection stored as a CSV file.
While it is not nearly as friendly as Excel (when Excel is not being unfriendly),
it provides a fairly independent means for manually adjusting CSV data.
Since it is scriptable, it can, in principle, be used for automated adjustment of CSV data too.

