/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

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
        /// <param name="fieldName">Name of field</param>
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
        /// <param name="fieldPosition">position of the field within the record</param>
        /// <returns>value associated with the field at the (numeric) position</returns>
        new
        TValue this[int fieldPosition] { get; set; }

        /// <summary>
        /// Try to lookup a field value, 
        /// if the value doesn't exist then 'false' is returned and a default value is passed back.
        /// </summary>
        /// <param name="fieldName">Name of field</param>
        /// <param name="outValue">
        /// on exit, when the return value is true, the value of the field,
        /// when the return value is false, the default value for <typeparamref name="TValue"/>
        /// </param>
        /// <returns>True if a value was found for the specified field name, False if the field was not found.</returns>
        bool TryGetValue(string fieldName, out TValue outValue);

    } // /interface
    

} // /namespace
