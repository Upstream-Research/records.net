/*  Copyright (c) 2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic, readonly, ordered field-value lookup interface for tabular data records.
    /// This interface is generic and covariant (unlike <see cref="IRecordAccessor{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">datatype for field values</typeparam>
    public interface IRecordViewer<out TValue>
    : IEnumerable<IFieldNameValuePair<TValue>>
    {
        /// <summary>
        /// Lookup a value associated with a field name.
        /// If the field does not exist, the the behavior is undefined.
        /// </summary>
        /// <param name="fieldName">Name of field</param>
        /// <remarks>
        /// It is recommended to throw System.Collections.Generic.KeyNotFoundException if the key does not exist
        /// (just like generic IDictionary), but client code should not make this assumption.
        /// Some implementations may return a null reference value.
        /// </remarks>
        TValue this[string fieldName] { get; }

        /// <summary>
        /// Get a field value by its order in the field list.
        /// </summary>
        /// <param name="fieldPosition">Position (aka. "index", aka. "ordinal") of the field</param>
        /// <returns>
        /// Field value.  If the position is invalid an ArgumentOutOfRangeException should be thrown.
        /// </returns>
        TValue this[int fieldPosition] { get; }
        
        /// <summary>
        /// Get the field position of a field by its name
        /// </summary>
        /// <param name="fieldName">Name of field to lookup.</param>
        /// <returns>-1 if field name is not found</returns>
        int IndexOfField(string fieldName);


        /// <summary>
        /// Get the number of fields in the record
        /// </summary>
        /// <remarks>
        /// This was defined as a method and not as a property
        /// so that types that derive from this, and which represent "strongly-typed" records
        /// can use properties exclusively for field values.
        /// </remarks>
        int GetFieldCount();

        /*
        /// <summary>
        /// Try to find a field (and its value)
        /// </summary>
        /// <param name="fieldName">Name of field to lookup</param>
        /// <returns>
        /// Null reference if the field cannot be found,
        /// Otherwise, returns an IFieldNameValuePair object that contains
        /// the name of the field and the field value.
        /// </returns>
        /// <remarks>
        /// This is defines a function that is supposed to be equivalent to <c>IDictionary{T}.TryGetValue()</c>,
        /// but which is covariant in the TValue type parameter.
        /// It is recommended that implementations implement this method in terms of their own TryGetValue() function when possible.
        /// </remarks>
        IFieldNameValuePair<TValue> FindField(string fieldName);
        */

        /*
        /// <summary>
        /// Get an enumerator of the field names for this record
        /// </summary>
        /// <returns></returns>
        IEnumerator<string> GetFieldNameEnumerator();
        */

    } // /interface
    

} // /namespace
