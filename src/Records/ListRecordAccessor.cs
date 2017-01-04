/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides a base record accessor implemented on basic arrays
    /// </summary>
    public class ListRecordAccessor<TValue> : IRecordAccessor<TValue>
    {
        private readonly IDictionary<string,int> _fieldOrdinalDictionary;
        private readonly IList<string> _fieldNameList;
        private IList<TValue> _fieldValueList;

        /// <summary>
        /// Create a record accessor that will use field names in the order they are found in a field name list,
        /// but that is not initially attached to any field value list.
        /// </summary>
        /// <param name="fieldNameList"></param>
        public ListRecordAccessor(
             IList<string> fieldNameList
            )
        {
            _fieldOrdinalDictionary = CreateFieldOrdinalDictionary(fieldNameList);
            _fieldNameList = fieldNameList;
        }

        /// <summary>
        /// Create a record accessor that will use field names found in a field name record
        /// and will get field values from a list.
        /// </summary>
        /// <param name="fieldNameList"></param>
        /// <param name="fieldValueList"></param>
        public ListRecordAccessor(
              IList<string> fieldNameList
             ,IList<TValue> fieldValueList
            )
            :this(fieldNameList)
        {
            _fieldValueList = fieldValueList;
        }

        /// <summary>
        /// Create a dictionary that can lookup field positions based on an ordered list of field names
        /// </summary>
        /// <param name="fieldNameList"></param>
        /// <returns></returns>
        private IDictionary<string,int> CreateFieldOrdinalDictionary(IList<string> fieldNameList)
        {
            if (null == fieldNameList)
            {
                throw new ArgumentNullException("fieldNameList");
            }
            int fieldCount = fieldNameList.Count;
            IDictionary<string,int> fieldOrdinalDictionary = new Dictionary<string,int>(fieldCount);
            int fieldOrdinal;
            string fieldName;
            for (fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
            {
                fieldName = fieldNameList[fieldOrdinal];
                fieldOrdinalDictionary[fieldName] = fieldOrdinal;
            }

            return fieldOrdinalDictionary;
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
            set
            {
                _fieldValueList = value;
            }
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
        /// Get a list of field names for the underlying dictionary
        /// </summary>
        /// <returns></returns>
        private IList<string> GetFieldNameList()
        {
            if (null == _fieldNameList)
            {
                throw new InvalidOperationException("Base Field Name List is not assigned");
            }

            return _fieldNameList;
        }

        private IDictionary<string,int> GetFieldOrdinalDictionary()
        {
            if (null == _fieldOrdinalDictionary)
            {
                throw new InvalidOperationException("Base Field Ordinal Dictionary is not assigned");
            }

            return _fieldOrdinalDictionary;
        }

        private int GetFieldOrdinal(string fieldName)
        {
            return GetFieldOrdinalDictionary()[fieldName];
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
                int fieldOrdinal = GetFieldOrdinal(fieldName);
                return GetFieldValueList()[fieldOrdinal];
            }

            set
            {
                int fieldOrdinal = GetFieldOrdinal(fieldName);
                GetFieldValueList()[fieldOrdinal] = value;
            }
        }

        /// <summary>
        /// Get/Set a field value by its position in the record.
        /// This is much more efficient than accessing the value by name.
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        public TValue this[int fieldOrdinal]
        {
            get
            {
                return GetFieldValueList()[fieldOrdinal];
            }

            set
            {
                GetFieldValueList()[fieldOrdinal] = value;
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
            int fieldOrdinal;
            bool wasFound = false;

            outFieldValue = default(TValue);
            if (GetFieldOrdinalDictionary().TryGetValue(fieldName, out fieldOrdinal))
            {
                outFieldValue = GetFieldValueList()[fieldOrdinal];
            }

            return wasFound;
        }

        /// <summary>
        /// Get the number of values in this record.
        /// </summary>
        public int FieldCount
        {
            get
            {
                int fieldNameCount = GetFieldOrdinalDictionary().Count;
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
        }

        /// <summary>
        /// Compare a field value to some other value according to the field domain rules
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue2"></param>
        /// <returns></returns>
        public int CompareFieldTo(string fieldName, TValue fieldValue2)
        {
            TValue fieldValue = this[fieldName];
            int resultNum = 1;

            if (null != fieldValue
                && null != fieldValue2
                )
            {
                // TODO 20160104 [db] TODO This is not a good way to compare field values,
                //  we need to know something more about the derived type of TValue.
                resultNum = Comparer<TValue>.Default.Compare(fieldValue, fieldValue2);
            }

            return resultNum;
        }

        /// <summary>
        /// Get an enumerator of the names of fields on this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            return GetFieldNameList().GetEnumerator();
        }


        /// <summary>
        /// Get an enumerator of field names and field values in this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            IList<TValue> fieldValueList = GetFieldValueList();
            IList<string> fieldNameList = GetFieldNameList();
            IEnumerator<TValue> fieldValueEnumerator = fieldValueList.GetEnumerator();
            IEnumerator<string> fieldNameEnumerator = fieldNameList.GetEnumerator();

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
