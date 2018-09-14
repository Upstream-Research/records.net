/*  Copyright (c) 2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements a record with no fields
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class EmptyRecordAccessor<TValue>
    : IRecordAccessor<TValue>
    {
        /// <summary>
        /// Create a new instance of a record accessor that has no fields.
        /// </summary>
        public EmptyRecordAccessor()
        {
        }

        /// <summary>
        /// Try to get a field value.  This will always fail since the record has no fields
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public TValue this[int fieldPosition]
        {
            get
            {
                throw new ArgumentOutOfRangeException("fieldPosition");
            }
            set
            {
                throw new ArgumentOutOfRangeException("fieldPosition");
            }
        }
        
        /// <summary>
        /// Try to get a field value by name.  This will always fail since the record has no fields.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public TValue this[string fieldName]
        {
            get
            {
                throw new KeyNotFoundException(String.Format("Unknown field: {0}", fieldName));
            }
            set
            {
                throw new KeyNotFoundException(String.Format("Unknown field: {0}", fieldName));
            }
        }

        /// <summary>
        /// Get the number of fields in the record.  
        /// This is always zero.
        /// </summary>
        /// <returns>0</returns>
        public int GetFieldCount()
        {
            return 0;
        }

        /// <summary>
        /// Try to get the position of a field in the record.
        /// This always returns -1, indicating that the field was not found
        /// since there are no fields in this record.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>-1</returns>
        public int IndexOfField(string fieldName)
        {
            return -1;
        }

        /// <summary>
        /// Try to get the value for a field.
        /// This will always return false.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outValue"></param>
        /// <returns>boolean false</returns>
        public bool TryGetValue(string fieldName, out TValue outValue)
        {
            outValue = default(TValue);
            return false;
        }

        /// <summary>
        /// Get an enumerator of all fields with their values.
        /// This will always be an empty enumerator since this record has no fields.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<TValue>> GetEnumerator()
        {
            IEnumerable<IFieldNameValuePair<TValue>> fieldInfoArray = new IFieldNameValuePair<TValue>[0];

            return fieldInfoArray.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // /class

} // /namespace
