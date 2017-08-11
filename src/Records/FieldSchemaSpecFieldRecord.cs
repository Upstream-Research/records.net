/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides access to meta-fields for a field parsed from a Field Schema Spec string.
    /// </summary>
    public class FieldSchemaSpecFieldRecord<TValue>
    : IRecordAccessor<object>
     , IRecordFieldType<TValue>
    {
        private const int FieldNamePosition = 0;
        private const int FieldTypeNamePosition = 1;
        private const int FieldDataTypePosition = 2;

        private string[] _metaFieldNameArray = new string[]
            {
                 "name"
                ,"type_name"
                ,"datatype"
            };
        private object[] _metaValueArray = new object[]
            {
                 null
                ,null
                ,null
            };
        private BasicRecordFieldType<TValue>  _baseFieldType;

        /// <summary>
        /// Create a new field description record for a field parsed from a field schema spec string
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldTypeName"></param>
        /// <param name="dataType"></param>
        internal FieldSchemaSpecFieldRecord(
             string fieldName
            ,string fieldTypeName
            ,Type dataType
            )
        {
            _baseFieldType = new BasicRecordFieldType<TValue>(dataType);
            
            _metaValueArray[FieldNamePosition] = fieldName;
            _metaValueArray[FieldTypeNamePosition] = fieldTypeName;
            _metaValueArray[FieldDataTypePosition] = dataType;
        }

        /// <summary>
        /// Get/Set a meta field value by its position
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public object this[int fieldPosition]
        {
            get
            {
                return _metaValueArray[fieldPosition];
            }

            set
            {
                object metaValue = value;
                _metaValueArray[fieldPosition] = metaValue;
            }
        }

        /// <summary>
        /// Get/Set a meta field value by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object this[string fieldName]
        {
            get
            {
                object fieldValue = null;
                int fieldPosition = FindMetaFieldPosition(fieldName);
                if (0 <= fieldPosition)
                {
                    fieldValue = _metaValueArray[fieldPosition];
                }

                return fieldValue;
            }

            set
            {
                object fieldValue = value;
                int fieldPosition = FindMetaFieldPosition(fieldName);
                if (0 <= fieldPosition)
                {
                    _metaValueArray[fieldPosition] = fieldValue;
                }
            }
        }

        /// <summary>
        /// Get the name of the field type as it was provided in the field schema spec
        /// </summary>
        public string FieldTypeName
        {
            get
            {
                return (string)_metaValueArray[FieldTypeNamePosition];
            }
        }

        /// <summary>
        /// Get the .NET datatype for field values belonging to the field described by this record
        /// </summary>
        public Type DataType
        {
            get
            {
                return (Type)_metaValueArray[FieldDataTypePosition];
            }
        }

        /// <summary>
        /// Get the number of meta fields in this record
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            throw new NotImplementedException();
        }

        private int 
        FindMetaFieldPosition(string searchFieldName)
        {
            int invalidPosition = -1;
            int foundFieldPosition = invalidPosition;
            int fieldPosition;

            fieldPosition = 0;
            while (fieldPosition < _metaFieldNameArray.Length
                && invalidPosition == foundFieldPosition
                )
            {
                string fieldName = _metaFieldNameArray[fieldPosition];
                if (MetaFieldNamesAreEqual(fieldName, searchFieldName))
                {
                    foundFieldPosition = fieldPosition;
                }
                fieldPosition += 1;
            }

            return foundFieldPosition;
        }

        private bool 
        MetaFieldNamesAreEqual(string fieldName1, string fieldName2)
        {
            return (0 == String.Compare(fieldName1, fieldName2, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Try to get the value of a metafield by name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out object fieldValue)
        {
            bool hasValue = false;
            int fieldPosition = FindMetaFieldPosition(fieldName);
            fieldValue = null;
            if (0 <= fieldPosition)
            {
                fieldValue = _metaValueArray[fieldPosition];
                hasValue = true;
            }

            return hasValue;
        }

        /// <summary>
        /// Determine if a field value is valid for storage in the field described by this record
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool IsValid(TValue fieldValue)
        {
            return _baseFieldType.IsValid(fieldValue);
        }

        /// <summary>
        /// Compare two values that belong to this field's domain
        /// </summary>
        /// <param name="fieldValue1"></param>
        /// <param name="fieldValue2"></param>
        /// <returns></returns>
        public int Compare(TValue fieldValue1, TValue fieldValue2)
        {
            return _baseFieldType.Compare(fieldValue1, fieldValue2);
        }

        /// <summary>
        /// Determine if two field values are equivalent in this field's domain
        /// </summary>
        /// <param name="fieldValue1"></param>
        /// <param name="fieldValue2"></param>
        /// <returns></returns>
        public bool Equals(TValue fieldValue1, TValue fieldValue2)
        {
            return _baseFieldType.Equals(fieldValue1, fieldValue2);
        }

        /// <summary>
        /// Get an equivalence hash code for comparing field values for the field described by this record
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public int GetHashCode(TValue fieldValue)
        {
            return _baseFieldType.GetHashCode(fieldValue);
        }

        /// <summary>
        /// Get an enumerator for the metafield names for this field
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            IEnumerable<string> metaFieldNameEnumeration = _metaFieldNameArray;

            return metaFieldNameEnumeration.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator of the metafields for this field
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            int fieldPosition = 0;

            while (fieldPosition < _metaFieldNameArray.Length
                && fieldPosition < _metaValueArray.Length
                )
            {
                string fieldName = _metaFieldNameArray[fieldPosition];
                object fieldValue = _metaValueArray[fieldPosition];

                yield return new KeyValuePair<string,object>(fieldName, fieldValue);

                fieldPosition += 1;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace

