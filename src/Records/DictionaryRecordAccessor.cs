/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IRecordAccessor over a .Net Dictionary
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TFieldType"></typeparam>
    public class DictionaryRecordAccessor<TValue, TFieldType> 
    : IRecordAccessorAdapter<TValue,IDictionary<string,TValue>>
    where TFieldType : IRecordFieldType<TValue>
    {
        private readonly IList<string> _fieldNameList;
        private IDictionary<string,TFieldType> _fieldTypeDictionary;
        private IDictionary<string,TValue> _baseDictionary;

        /// <summary>
        /// Create a record accessor that is not attached to any record data
        /// and that will use field names in the order provided.
        /// </summary>
        public DictionaryRecordAccessor(
            IList<string> fieldNameList
            ,IDictionary<string,TFieldType> fieldComparerDictionary = null
            )
        {
            if (null == fieldNameList)
            {
                throw new ArgumentNullException("fieldNameList");
            }

            _fieldNameList = fieldNameList;
            _fieldTypeDictionary = fieldComparerDictionary;
        }

        /// <summary>
        /// Create a record accessor and attach it to a dictionary of record data.
        /// If no (ordered) field name list is provided, 
        /// then the order of field names will be determined by 
        /// the order that field name keys are returned from the input dictionary
        /// </summary>
        /// <param name="baseDictionary"></param>
        public DictionaryRecordAccessor(
            IDictionary<string,TValue> baseDictionary
            ,IList<string> fieldNameList = null
            ,IDictionary<string,TFieldType> fieldComparerDictionary = null
            )
        {
            if (null == baseDictionary)
            {
                throw new ArgumentNullException("baseDictionary");
            }
            _fieldNameList = fieldNameList;
            if (null == _fieldNameList)
            {
                _fieldNameList = new List<string>(baseDictionary.Keys);
            }
            _fieldTypeDictionary = fieldComparerDictionary;

            AttachTo(baseDictionary);
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
        }

        /// <summary>
        /// Attach the dictionary record accessor to a new dictionary of field values
        /// </summary>
        /// <param name="fieldValueDictionary"></param>
        public void AttachTo(IDictionary<string,TValue> fieldValueDictionary)
        {
            _baseDictionary = fieldValueDictionary;
        }

        /// <summary>
        /// Get the base dictionary, throw an exception if it is not set
        /// </summary>
        /// <returns></returns>
        private IDictionary<string,TValue> GetFieldValueDictionary()
        {
            if (null == FieldValueDictionary)
            {
                throw new InvalidOperationException("Dictionary record accessor is not attached to a dictionary");
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
        /// Determine if a field value can be assigned to the specified field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private bool FieldValueIsValid(string fieldName, TValue fieldValue)
        {
            bool isValid = true;
            TFieldType fieldType;
            
            if (null != _fieldTypeDictionary
                && _fieldTypeDictionary.TryGetValue(fieldName, out fieldType)
                )
            {
                if (null != fieldType)
                {
                    isValid = fieldType.IsValid(fieldValue);
                }
            }

            return isValid;
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
                TValue fieldValue = value;
                if (FieldValueIsValid(fieldName, fieldValue))
                {
                    GetFieldValueDictionary()[fieldName] = fieldValue;
                }
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
                TValue fieldValue = value;
                string fieldName = GetFieldName(fieldOrdinal);
                if (FieldValueIsValid(fieldName, fieldValue))
                {
                    GetFieldValueDictionary()[fieldName] = fieldValue;
                }
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
                if (null != _fieldTypeDictionary)
                {
                    TFieldType fieldType2;
                    if (_fieldTypeDictionary.TryGetValue(fieldName, out fieldType2))
                    {
                        IComparer<TValue> sortComparer2 = fieldType2 as IComparer<TValue>;
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
