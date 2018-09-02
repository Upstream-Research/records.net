/*  Copyright (c) 2016 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides a base record accessor implemented on basic arrays
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TFieldType"></typeparam>
    public class ListRecordAccessor<TValue, TFieldType> 
    : IRecordAccessorAdapter<TValue,IList<TValue>>
    where TFieldType : IRecordFieldType<TValue>
    {
        private readonly IRecordSchemaAccessor<TFieldType> _recordSchema;
        private IList<TValue> _fieldValueList;

        /// <summary>
        /// Create a record accessor that will use an underlying list to access field values
        /// corresponding to a field schema.
        /// The underlying list must be "attached" using the AttachTo() method.
        /// </summary>
        /// <param name="recordSchema"></param>
        public ListRecordAccessor(
              IRecordSchemaAccessor<TFieldType> recordSchema
            )
        {
            if (null == recordSchema)
            {
                throw new ArgumentNullException("recordSchema");
            }

            _recordSchema = recordSchema;
        }

        /// <summary>
        /// Create a record accessor that will use an underlying list to access field values
        /// corresponding to a field schema.
        /// </summary>
        /// <param name="recordSchema"></param>
        /// <param name="fieldValueList"></param>
        public ListRecordAccessor(
              IRecordSchemaAccessor<TFieldType> recordSchema
             ,IList<TValue> fieldValueList
            )
            :this(recordSchema)
        {
            AttachTo(fieldValueList);
        }

        /// <summary>
        /// Get the record schema associated with this record
        /// </summary>
        private IRecordSchemaAccessor<TFieldType> RecordSchema
        {
            get
            {
                return _recordSchema;
            }
        }

        /// <summary>
        /// Get/Set the underlying field value list
        /// </summary>
        public IList<TValue> FieldValueList
        {
            get
            {
                return _fieldValueList;
            }
        }

        /// <summary>
        /// Attach the list record accessor to a list of values
        /// </summary>
        /// <param name="fieldValueList"></param>
        public void AttachTo(IList<TValue> fieldValueList)
        {
            _fieldValueList = fieldValueList;
        }

        /// <summary>
        /// Get the underlying field value list, raise an exception if the underlying list is not set.
        /// </summary>
        /// <returns></returns>
        private IList<TValue> GetFieldValueList()
        {
            if (null == _fieldValueList)
            {
                throw new InvalidOperationException("Base FieldValueList is not assigned");
            }

            return _fieldValueList;
        }

        /// <summary>
        /// Get the offset of a field in the record
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int IndexOfField(string fieldName)
        {
            return RecordSchema.IndexOfField(fieldName);
        }

        /// <summary>
        /// Check that a field value is valid to store in the specified field
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool FieldValueIsValid(int fieldPosition, TValue fieldValue)
        {
            bool isValid = true;  // valid unless the field type disagrees

            if (fieldPosition < RecordSchema.GetFieldCount())
            {
                TFieldType fieldType = RecordSchema[fieldPosition];
                if (null != fieldType)
                {
                    isValid = fieldType.IsValid(fieldValue);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Get a field value from the underlying list that exists at the same position
        /// as found in the field name list provided at construction time
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public TValue this[string fieldName]
        {
            get
            {
                int fieldPosition = IndexOfField(fieldName);
                return GetFieldValueList()[fieldPosition];
            }

            set
            {
                TValue fieldValue = value;
                int fieldPosition = IndexOfField(fieldName);
                if (FieldValueIsValid(fieldPosition, fieldValue))
                {
                    GetFieldValueList()[fieldPosition] = fieldValue;
                }
            }
        }

        /// <summary>
        /// Get/Set a field value by its position in the record.
        /// This is much more efficient than accessing the value by name.
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public TValue this[int fieldPosition]
        {
            get
            {
                return GetFieldValueList()[fieldPosition];
            }

            set
            {
                TValue fieldValue = value;
                if (FieldValueIsValid(fieldPosition, fieldValue))
                {
                    GetFieldValueList()[fieldPosition] = fieldValue;
                }
            }
        }

        /// <summary>
        /// Try to get the value for a key in the dictionary
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outFieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out TValue outFieldValue)
        {
            int fieldPosition;
            bool wasFound = false;

            outFieldValue = default(TValue);
            fieldPosition = IndexOfField(fieldName);
            if (0 <= fieldPosition)
            {
                outFieldValue = GetFieldValueList()[fieldPosition];
                wasFound = true;
            }

            return wasFound;
        }

        /// <summary>
        /// Get the number of values in this record.
        /// </summary>
        public int GetFieldCount()
        {
            int fieldNameCount = RecordSchema.GetFieldCount();
            int fieldValueCount = 0;
            int fieldCount = 0;
                
            if (null != FieldValueList)
            {
                fieldValueCount = FieldValueList.Count;
            }
            fieldCount = fieldValueCount;
            if (fieldCount > fieldNameCount)
            {
                fieldCount = fieldNameCount;
            }

            return fieldCount;
        }

        /// <summary>
        /// Compare a field value to some other value according to the field domain rules
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue2"></param>
        /// <returns></returns>
        public int CompareFieldTo(string fieldName, TValue fieldValue2)
        {
            IComparer<TValue> sortComparer = Comparer<TValue>.Default;
            TValue fieldValue = this[fieldName];
            int fieldPosition;
            int resultNum = 0;
            

            if (null == fieldValue
                && null == fieldValue2
                )
            {
                resultNum = 0;
            }
            else if (null == fieldValue
                && null != fieldValue2
                )
            {
                resultNum = -1;
            }
            else if (null != fieldValue
                && null == fieldValue2
                )
            {
                resultNum = 1;
            }
            else if (null != fieldValue
                && null != fieldValue2
                )
            {
                IComparer<TValue> sortComparer2 = null;
                fieldPosition = IndexOfField(fieldName);
                if (0 <= fieldPosition
                    && RecordSchema.GetFieldCount() > fieldPosition
                    )
                {
                    sortComparer2 = RecordSchema[fieldPosition];
                }
                if (null != sortComparer2)
                {
                    sortComparer = sortComparer2;
                }

                resultNum = sortComparer.Compare(fieldValue, fieldValue2);
            }

            return resultNum;
        }

        /// <summary>
        /// Get an enumerator of the names of fields on this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            return RecordSchema.GetFieldNameEnumerator();
        }


        /// <summary>
        /// Get an enumerator of field names and field values in this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            IList<TValue> fieldValueList = GetFieldValueList();
            IEnumerator<TValue> fieldValueEnumerator = fieldValueList.GetEnumerator();
            IEnumerator<string> fieldNameEnumerator = GetFieldNameEnumerator();

            while (fieldValueEnumerator.MoveNext()
                && fieldNameEnumerator.MoveNext()
                )
            {
                string fieldName = fieldNameEnumerator.Current;
                TValue fieldValue = fieldValueEnumerator.Current;
                yield return new KeyValuePair<string,TValue>(fieldName,fieldValue);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // /class
} // /namespace
