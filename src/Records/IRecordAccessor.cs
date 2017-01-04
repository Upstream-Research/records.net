/*  Copyright (c) 2016 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections.Generic;

using Upstream.System.Collections;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a basic field-value lookup interface for tabular data records.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IRecordAccessor<TValue> : INameValueLookup<TValue>
    {
        /// <summary>
        /// Get a field value by its order in the field list.
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        TValue this[int fieldOrdinal] { get; set; }

        /// <summary>
        /// Compare the value of a field in this record to some field value.
        /// Comparison is done according to the field's domain (i.e. collation rules)
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue2"></param>
        /// <returns>0 if the values are equivalent,
        /// negative if this field value sorts less than the input value,
        /// postive if this field vlaue sorts greater than the input value.
        /// positive if either of the field values is null (null is not equal to null).
        /// </returns>
        int CompareFieldTo(string fieldName, TValue fieldValue2);

        /// <summary>
        /// Get the number of fields in the record
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Get an enumerator of the field names for this record
        /// </summary>
        /// <returns></returns>
        IEnumerator<string> GetFieldNameEnumerator();
    } // /interface
    

} // /namespace
