/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines an object that provides access to the field metadata for a record collection
    /// </summary>
    /// <typeparam name="TFieldType"></typeparam>
    public interface IRecordSchemaAccessor<TFieldType>
    : IRecordAccessor<TFieldType>
    {
        /// <summary>
        /// Get the field position of a field by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>-1 if field name is not found</returns>
        int IndexOfField(string fieldName);

        /// <summary>
        /// Get the name of a field at a position.
        /// Note, it is generally more efficient to enumerate the fields whenever possible.
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        string FieldNameAt(int fieldPosition);

    } // /interface
} // /namespace
