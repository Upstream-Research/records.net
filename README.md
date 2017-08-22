records.net
-----------

.NET Tabular Data and Data Record Library

*Copyright (c) 2017 Upstream Research, All Rights Reserved.*

The source code for this library is made available under an "MIT" license,
See the "LICENSE" file for details.

Revised: 2017-08-14 (db)

_(20170810 (db) Warning, this is unstable and untested code.
It has been released in the interest of getting this code out into the community,
where it may be further developed into something stable)_


## Overview

This is a data support library intended to provide access to tabular/record data similar to ADO.NET (i.e. System.Data).

The goals are to provide a finer set of core interfaces than System.Data
as well as to provide a .NET Standard implementation for basic tabular data access.

This library provides an alternative and a complement to ORM (Object/Relational Mapping) frameworks.
It is an alternative because it does not try to hide the tabular nature of relational data,
yet it is complementary because one can build an ORM framework on this library.

One of the strengths intended for this library is to provide a level of abstraction for tabular data,
which allows for simpler transmission, transformation, and encoding of records
at the expense of strongly-typed "entity" objects 
(although strongly-typed records are still possible with this library).


## Some Remarks On Existing Data Libraries and Frameworks

ADO.NET provides two different groups of objects: 

1. Data I/O objects, such as `IDataReader`
2. Data structure objects, such as `DataTable`

These are mostly independent groups; perhaps they should have been placed in separate namespaces.

In .NET Core 1.0, Microsoft considered `System.Data` "unsupported"
and yet provided some implementation of the data I/O objects.
It appears that `System.Data` will be supported in .NET Core 2.0 and .NET Standard 2.0.

ORM libraries rely on on "Plain Old Objects", or "Entity" objects
which attempt to encode record fields as strongly-typed object properties.
This is often very convenient, but it comes at the expense of generality;
to handle such entity objects more generally, 
complicated systems involving reflection and collection manipulation have been devised.


## Core Record Objects

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

Presently, this project does not use a unit testing framework.
The approach to testing taken here is to develop light-weight console programs
that attempt to reach a high level of "code coverage"
without the introduction of extra dependencies.
Testing the code involves invoking the test console program(s) repeatedly with different arguments.
Validating the code's behavior involves comparing the output of the programs with an expected output.
This approach is, admittedly unconventional and bound to evoke frowny faces,
but the author believes there are several benefits to this approach that justify giving it a try.
That said, the test code at the time of writing this isn't very good, 
but improvements aren't difficult to imagine.
There has been little time for finishing the library code, much less testing it.
