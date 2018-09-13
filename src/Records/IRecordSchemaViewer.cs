/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines an object that provides access to the field metadata for a record collection
    /// </summary>
    /// <typeparam name="TFieldType"></typeparam>
    public interface IRecordSchemaViewer<out TFieldType>
    : IRecordViewer<TFieldType>
    {
        
        /// <summary>
        /// Get an enumerable collection of the field names associated with records in this schema
        /// </summary>
        IEnumerable<string> FieldNames { get; }

        /// <summary>
        /// Get the name of a field at a position.
        /// Note, it is generally more efficient to enumerate the fields whenever possible.
        /// If the field position is out of range, then an ArgumentOutOfRangeException should be raised.
        /// </summary>
        /// <param name="fieldPosition">
        /// Position of field
        /// </param>
        /// <returns>
        /// String name of field.  Should not return a null reference.
        /// </returns>
        string FieldNameAt(int fieldPosition);

    } // /interface
} // /namespace
