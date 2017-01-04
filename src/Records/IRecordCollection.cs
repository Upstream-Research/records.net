/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a collection type for record accessors.
    /// Doesn't implement ICollection, but implements a subset of it.
    /// Enumerating this collection does not necessarily return distinct objects
    /// since an IRecordAccessor is a "visitor".
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IRecordCollection<TValue> 
    : IEnumerable<IRecordAccessor<TValue>>
    {
        /// <summary>
        /// Get the number of records in the collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get the number of fields in the record
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Get an enumeration of the field names for this record
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> FieldNames { get; }
        
        /// <summary>
        /// Delete all records in this collection
        /// </summary>
        void Clear();

        /// <summary>
        /// Create a new record reader that will scan this collection
        /// </summary>
        /// <returns></returns>
        IRecordCollectionReader<TValue> OpenRecordCollectionReader();

        /// <summary>
        /// Create a new record writer that will append new records to this collection
        /// </summary>
        /// <returns></returns>
        IRecordCollectionWriter<TValue> OpenRecordCollectionWriter();

    } // /interface

} // /namespace
