/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;  // KeyValuePair

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic field-value lookup interface for tabular data records.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IRecordAccessor<TValue>
    : IEnumerable<KeyValuePair<string,TValue>>
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
        TValue this[string fieldName] { get; set; }

        /// <summary>
        /// Get a field value by its order in the field list.
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        TValue this[int fieldOrdinal] { get; set; }

        /// <summary>
        /// Try to lookup a field value, 
        /// if the value doesn't exist then 'false' is returned and a default value is passed back.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outValue"></param>
        /// <returns>True if a value was found for the specified field name, False if the field was not found.</returns>
        bool TryGetValue(string fieldName, out TValue outValue);

        /// <summary>
        /// Compare the value of a field in this record to some field value.
        /// Comparison is done according to the field's domain (i.e. collation rules)
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue2"></param>
        /// <returns>0 if the values are equivalent,
        /// negative if this field value sorts less than the input value,
        /// postive if this field vlaue sorts greater than the input value.
        /// null is equivalent to null.
        /// null should compare less than non-null.
        /// </returns>
        int CompareFieldTo(string fieldName, TValue fieldValue2);

        /// <summary>
        /// Get the number of fields in the record
        /// </summary>
        int GetFieldCount();

        /// <summary>
        /// Get an enumerator of the field names for this record
        /// </summary>
        /// <returns></returns>
        IEnumerator<string> GetFieldNameEnumerator();

    } // /interface
    

} // /namespace
