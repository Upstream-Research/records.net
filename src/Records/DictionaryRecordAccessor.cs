/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IRecordAccessor over a .Net Dictionary
    /// </summary>
    public class DictionaryRecordAccessor<TValue> : IRecordAccessor<TValue>
    {
        private readonly IList<string> _fieldNameList;
        private IDictionary<string,IComparer<TValue>> _fieldComparerDictionary;
        private IDictionary<string,TValue> _baseDictionary;

        /// <summary>
        /// Create a record accessor that is not attached to any record data
        /// and that will use field names in the order provided.
        /// </summary>
        public DictionaryRecordAccessor(
            IList<string> fieldNameList
            ,IDictionary<string,IComparer<TValue>> fieldComparerDictionary
            )
        {
            if (null == fieldNameList)
            {
                throw new ArgumentNullException("fieldNameList");
            }

            _fieldNameList = fieldNameList;
            _fieldComparerDictionary = fieldComparerDictionary;
        }

        /// <summary>
        /// Create a record accessor and attach it to a dictionary of record data.
        /// The order of field names will be determined by the order that field
        /// name keys are returned from the input dictionary
        /// </summary>
        /// <param name="baseDictionary"></param>
        public DictionaryRecordAccessor(
            IDictionary<string,TValue> baseDictionary
            )
        {
            if (null == baseDictionary)
            {
                throw new ArgumentNullException("baseDictionary");
            }
            FieldValueDictionary = baseDictionary;
            _fieldNameList = new List<string>(baseDictionary.Keys);
        }

        /// <summary>
        /// Get the base dictionary of record data.
        /// Set the base dictionary to which this record accessor is attached.
        /// A newly set dictionary must have the same fields defined during construction of this object.
        /// </summary>
        public IDictionary<string,TValue> FieldValueDictionary
        {
            get
            {
                return _baseDictionary;
            }
            set
            {
                _baseDictionary = value;
            }
        }

        /// <summary>
        /// Get the base dictionary, throw an exception if it is not set
        /// </summary>
        /// <returns></returns>
        private IDictionary<string,TValue> GetFieldValueDictionary()
        {
            if (null == FieldValueDictionary)
            {
                throw new InvalidOperationException("BaseDictionary is not assigned");
            }

            return FieldValueDictionary;
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

        /// <summary>
        /// Get the position associated with the given field name
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        private string GetFieldName(int fieldOrdinal)
        {
            return GetFieldNameList()[fieldOrdinal];
        }

        /// <summary>
        /// Get/Set the value stored under a given field name.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public TValue this[string fieldName]
        {
            get
            {
                return GetFieldValueDictionary()[fieldName];
            }
            set
            {
                GetFieldValueDictionary()[fieldName] = value;
            }
        }

        /// <summary>
        /// Get a field value by its position in the dictionary.
        /// The position in a dictionary is not well defined, 
        /// this property implements it by keeping a list of dictionary keys for fast lookup.
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        public TValue this[int fieldOrdinal]
        {
            get
            {
                string fieldName = GetFieldName(fieldOrdinal);
                return GetFieldValueDictionary()[fieldName];
            }
            set
            {
                string fieldName = GetFieldName(fieldOrdinal);
                GetFieldValueDictionary()[fieldName] = value;
            }
        }

        /// <summary>
        /// Try to find the value for a field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outFieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out TValue outFieldValue)
        {
            return GetFieldValueDictionary().TryGetValue(fieldName, out outFieldValue);
        }

        /// <summary>
        /// Get the number of fields in the underlying record
        /// </summary>
        public int GetFieldCount()
        {
            return GetFieldValueDictionary().Count;
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
                if (null != _fieldComparerDictionary)
                {
                    IComparer<TValue> sortComparer2 = null;
                    if (_fieldComparerDictionary.TryGetValue(fieldName, out sortComparer2))
                    {
                        if (null != sortComparer2)
                        {
                            sortComparer = sortComparer2;
                        }
                    }
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
            return GetFieldNameList().GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator of the fields and values for the record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            IDictionary<string,TValue> fieldValueDictionary = GetFieldValueDictionary();
            IList<string> fieldNameList = GetFieldNameList();
            int fieldCount = fieldNameList.Count;
            int fieldOrdinal;
            string fieldName;
            TValue fieldValue;
            for (fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
            {
                fieldName = fieldNameList[fieldOrdinal];
                fieldValue = fieldValueDictionary[fieldName];
                yield return new KeyValuePair<string,TValue>(fieldName,fieldValue);
            }
        }

        /// <summary>
        /// Implements non-generic Enumerator to get an enumeration of generic KeyValue pairs
        /// of field names and field values.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // /class
} // /namespace
