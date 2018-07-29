/*  Copyright (c) 2017 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic interface which RecordFieldProperties implementations should implement
    /// </summary>
    public interface IRecordFieldType<in TValue>
    : IComparer<TValue>
     ,IEqualityComparer<TValue>
    {
        /// <summary>
        /// Get the datatype for all values in this field
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// Determine if a value is a valid (in the domain) for this field
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        bool IsValid(TValue fieldValue);
    } // /interface
} // /namespace
