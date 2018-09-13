/*  Copyright (c) 2016 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

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
        private readonly IRecordSchemaViewer<TFieldType> _recordSchema;
        private IDictionary<string,TValue> _baseDictionary;

        /// <summary>
        /// Create a record accessor that is not attached to any record data.
        /// and that will use field names in the order provided.
        /// </summary>
        /// <param name="recordSchema"></param>
        public DictionaryRecordAccessor(
            IRecordSchemaViewer<TFieldType> recordSchema
            )
        {
            if (null == recordSchema)
            {
                throw new ArgumentNullException("recordSchema");
            }

            _recordSchema = recordSchema;
        }

        /// <summary>
        /// Create a record accessor and attach it to a dictionary of record data.
        /// </summary>
        /// <param name="recordSchema"></param>
        /// <param name="baseDictionary"></param>
        public DictionaryRecordAccessor(
            IRecordSchemaViewer<TFieldType> recordSchema
            ,IDictionary<string,TValue> baseDictionary
            )
            : this(recordSchema)
        {
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
        /// Get the record schema associated with this record
        /// </summary>
        private IRecordSchemaViewer<TFieldType> RecordSchema
        {
            get
            {
                return _recordSchema;
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
                throw new InvalidOperationException("Dictionary record accessor is not attached to a dictionary");
            }

            return FieldValueDictionary;
        }
        
        /// <summary>
        /// Get the position associated with the given field name
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        private string GetFieldName(int fieldPosition)
        {
            return RecordSchema.FieldNameAt(fieldPosition);
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
            int fieldPosition = RecordSchema.IndexOfField(fieldName);
            
            if (0 <= fieldPosition)
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
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public TValue this[int fieldPosition]
        {
            get
            {
                string fieldName = GetFieldName(fieldPosition);
                return GetFieldValueDictionary()[fieldName];
            }
            set
            {
                TValue fieldValue = value;
                string fieldName = GetFieldName(fieldPosition);
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
        /// Try to find a field and its value by the field name.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>null reference if the field is not found</returns>
        private IFieldNameValuePair<TValue> FindField(string fieldName)
        {
            IFieldNameValuePair<TValue> fieldItem = null;
            TValue fieldValue;

            if (TryGetValue(fieldName, out fieldValue))
            {
                fieldItem = new FieldNameValuePair<TValue>(fieldName, fieldValue);
            }

            return fieldItem;
        }

        /// <summary>
        /// Get the position of a field by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int IndexOfField(string fieldName)
        {
            return RecordSchema.IndexOfField(fieldName);
        }

        /// <summary>
        /// Get the number of fields in the underlying record
        /// </summary>
        public int GetFieldCount()
        {
            return RecordSchema.GetFieldCount();
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
                TFieldType fieldType2 = RecordSchema[fieldName];
                IComparer<TValue> sortComparer2 = fieldType2 as IComparer<TValue>;
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
            return RecordSchema.FieldNames.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator of the fields and values for the record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<TValue>> GetEnumerator()
        {
            IDictionary<string,TValue> fieldValueDictionary = GetFieldValueDictionary();
            IEnumerator<string> fieldNameEnumerator = GetFieldNameEnumerator();
            string fieldName;
            TValue fieldValue;
            while (fieldNameEnumerator.MoveNext())
            {
                fieldName = fieldNameEnumerator.Current;
                fieldValue = fieldValueDictionary[fieldName];
                yield return new FieldNameValuePair<TValue>(fieldName,fieldValue);
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
