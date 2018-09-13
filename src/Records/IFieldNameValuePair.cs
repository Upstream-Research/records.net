/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a readonly, covariant generic interface of a KeyValuePair with a string key
    /// for use in IRecordViewer.
    /// </summary>
    /// <typeparam name="TValue">type of value associated with the named record field</typeparam>
    public interface IFieldNameValuePair<out TValue>
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Value assigned to the field
        /// </summary>
        TValue Value { get; }
    } // /interface

} // /namespace
