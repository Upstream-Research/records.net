/*  Copyright (c) 2016 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;  // KeyValuePair

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic field-value lookup interface for tabular data records.
    /// </summary>
    /// <typeparam name="TValue">datatype for field values</typeparam>
    public interface IRecordAccessor<TValue>
    : IRecordViewer<TValue>
    {
        /// <summary>
        /// Lookup a value associated with a field name.
        /// If the field does not exist, the the behavior is undefined.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <remarks>
        /// Should raise System.Collections.Generic.KeyNotFoundException if the key does not exist
        /// (just like generic IDictionary), but client code should not make this assumption.
        /// Some implementations may return a null reference value.
        /// </remarks>
        new 
        TValue this[string fieldName] { get; set; }

        /// <summary>
        /// Get a field value by its order in the field list.
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        new
        TValue this[int fieldPosition] { get; set; }

        /// <summary>
        /// Try to lookup a field value, 
        /// if the value doesn't exist then 'false' is returned and a default value is passed back.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outValue"></param>
        /// <returns>True if a value was found for the specified field name, False if the field was not found.</returns>
        bool TryGetValue(string fieldName, out TValue outValue);

    } // /interface
    

} // /namespace
