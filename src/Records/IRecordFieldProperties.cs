/*  Copyright (c) 2017 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic interface which RecordFieldProperties implementations should implement
    /// </summary>
    public interface IRecordFieldProperties<TValue>
    {
        /// <summary>
        /// Get the datatype for all values in this field
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// Get an IComparer that can compare two field values for this field to determine their sort order
        /// </summary>
        IComparer<TValue> ValueSortComparer { get; }

        /// <summary>
        /// Get an IEqualityComparer that can compute hash codes for field values on this field
        /// </summary>
        IEqualityComparer<TValue> ValueEqualityComparer { get; }

    } // /interface
} // /namespace
