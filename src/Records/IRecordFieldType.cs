/*  Copyright (c) 2017 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic interface which Record Schema Field Type implementations should implement.
    /// </summary>
    /// <remarks>
    /// Notice that this interface inherits <see cref="IComparer{TValue}"/> and <see cref="IEqualityComparer{TValue}"/>,
    /// these interfaces provide for sorting and hashing of field values.
    /// </remarks>
    public interface IRecordFieldType<in TValue>
    : IComparer<TValue>
     ,IEqualityComparer<TValue>
    {
        /// <summary>
        /// Basic datatype associated with values in this field.
        /// </summary>
        Type SystemType { get; }

        /// <summary>
        /// Determine if a value is valid (i.e. in the domain) for this field
        /// </summary>
        /// <param name="fieldValue">A field value to test for validity</param>
        /// <returns>True if the field value is within the field's domain.</returns>
        bool IsValid(TValue fieldValue);
    } // /interface
} // /namespace
