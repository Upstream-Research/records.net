/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IRecordSchemaAccessor in a very basic way.
    /// </summary>
    public class BasicRecordSchema<TFieldType>
    : IRecordSchemaViewer<TFieldType>
    {
        // list of field names for fields described in the schema
        private IList<string> _fieldNameList;
        // list of type information objects for fields described in the schema
        private IList<TFieldType> _fieldTypeList;
        // dictionary to allow quick lookup of position of field information in lists based on field name
        private IDictionary<string,int> _fieldPositionDictionary;

        /// <summary>
        /// Create a new, empty record schema.
        /// New field definitions can be added using the AddField() method.
        /// </summary>
        public BasicRecordSchema()
        {
            const int fieldCountEstimate = 16;

            _fieldNameList = new List<string>(fieldCountEstimate);
            _fieldTypeList = new List<TFieldType>(fieldCountEstimate);
            _fieldPositionDictionary = new Dictionary<string,int>(fieldCountEstimate);
        }

        private IList<string> FieldNameList
        {
            get
            {
                return _fieldNameList;
            }
        }

        /// <summary>
        /// Get the field names as an enumerable collection
        /// </summary>
        public IEnumerable<string> FieldNames
        {
            get
            {
                return FieldNameList;
            }
        }

        private IList<TFieldType> FieldTypeList
        {
            get
            {
                return _fieldTypeList;
            }
        }

        private IDictionary<string,int> FieldPositionDictionary
        {
            get
            {
                return _fieldPositionDictionary;
            }
        }

        /// <summary>
        /// Get a Field Type object for a field by the field position
        /// </summary>
        /// <param name="fieldPosition">
        /// Position of a field.
        /// </param>
        /// <returns>
        /// Field type for the specified field.
        /// </returns>
        public TFieldType this[int fieldPosition]
        {
            get
            {
                return FieldTypeList[fieldPosition];
            }
        }

        /// <summary>
        /// Get a Field Type object for a field by the field name
        /// </summary>
        /// <param name="fieldName">
        /// Name of a field
        /// </param>
        /// <returns>
        /// Field type associated with the field name
        /// </returns>
        public TFieldType this[string fieldName]
        {
            get
            {
                int fieldPosition = IndexOfField(fieldName);
                if (0 > fieldPosition
                    && FieldTypeList.Count <= fieldPosition
                    )
                {
                    throw new KeyNotFoundException(String.Format("Specified field was not found: {0}", fieldName));
                }

                return FieldTypeList[fieldPosition];
            }
        }

        /// <summary>
        /// Get the number of fields in this record scheme
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return FieldNameList.Count;
        }

        /// <summary>
        /// Lookup a field's position by its name
        /// </summary>
        /// <param name="fieldName">
        /// Name of field to lookup.
        /// </param>
        /// <returns>
        /// -1 if the field name is out of range.
        /// </returns>
        public int IndexOfField(string fieldName)
        {
            const int invalidFieldPosition = -1;
            int fieldPosition;

            if (false == FieldPositionDictionary.TryGetValue(fieldName, out fieldPosition))
            {
                fieldPosition = invalidFieldPosition;
            }

            return fieldPosition;
        }

        /// <summary>
        /// Get the name of a field by its position in the field list.
        /// Throws <see cref="ArgumentOutOfRangeException"/>  if the field position is invalid.
        /// </summary>
        /// <param name="fieldPosition">
        /// Position of a field.
        /// </param>
        /// <returns>
        /// Name of the field found.
        /// </returns>
        public string 
        FieldNameAt(int fieldPosition)
        {
            if (0 > fieldPosition)
            {
                throw new ArgumentOutOfRangeException("fieldPosition");
            }
            else if (FieldNameList.Count <= fieldPosition)
            {
                throw new ArgumentOutOfRangeException("fieldPosition");
            }

            return FieldNameList[fieldPosition];
        }

        /// <summary>
        /// Get an enumerator of the field names
        /// </summary>
        /// <returns>Enumerator of field name strings</returns>
        public IEnumerator<string> 
        GetFieldNameEnumerator()
        {
            return FieldNameList.GetEnumerator();
        }

        /// <summary>
        /// Try to get a Field Type object by its field name
        /// </summary>
        /// <param name="fieldName">Name of the field to lookup</param>
        /// <param name="outValue">Field type information for the field</param>
        /// <returns>
        /// True if the field type information was found for the named field.
        /// False otherwise.
        /// </returns>
        public bool TryGetValue(string fieldName, out TFieldType outValue)
        {
            bool wasFound = false;
            int fieldPosition;

            outValue = default(TFieldType);
            fieldPosition = IndexOfField(fieldName);
            if (0 <= fieldPosition
                && FieldTypeList.Count > fieldPosition
                )
            {
                outValue = FieldTypeList[fieldPosition];
                wasFound = true;
            }

            return wasFound;
        }

        /// <summary>
        /// Try to find a field and its type by the field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>null reference if the field was not found,
        /// Otherwise returns an IFieldNameValuePair which contains the field type
        /// </returns>
        private IFieldNameValuePair<TFieldType> FindField(string fieldName)
        {
            IFieldNameValuePair<TFieldType> fieldItem = null;
            TFieldType fieldType;
            
            if (TryGetValue(fieldName, out fieldType))
            {
                fieldItem = new FieldNameValuePair<TFieldType>(fieldName, fieldType);
            }

            return fieldItem;
        }

        /// <summary>
        /// Get an dictionary enumerator of the field names and their types
        /// </summary>
        /// <returns>
        /// Enumerator of fields in the schema, 
        /// each field is encoded in a KeyValuePair containing the field name and its type information
        /// </returns>
        public IEnumerator<IFieldNameValuePair<TFieldType>> 
        GetEnumerator()
        {
            for(int fieldPosition = 0; fieldPosition < FieldNameList.Count; fieldPosition++)
            {
                string fieldName = FieldNameList[fieldPosition];
                TFieldType fieldType = FieldTypeList[fieldPosition];

                yield return new FieldNameValuePair<TFieldType>(fieldName, fieldType);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a new field to the record schema.
        /// If the field name is already in the schema, an exception will be raised.
        /// </summary>
        /// <param name="fieldName">Name of field to be added</param>
        /// <param name="fieldType">Type information for field to be added</param>
        public void AddField(string fieldName, TFieldType fieldType)
        {
            if (null == fieldName)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (FieldPositionDictionary.ContainsKey(fieldName))
            {
                throw new ArgumentException(
                     String.Format("field '{0}' already exists in the record schema", fieldName)
                    ,"fieldName"
                    );
            }

            FieldPositionDictionary.Add(fieldName, FieldNameList.Count);
            FieldNameList.Add(fieldName);
            FieldTypeList.Add(fieldType);
        }
        
        /// <summary>
        /// Add a new field to the record schema.
        /// If the field name is already in the schema, an exception will be raised.
        /// </summary>
        /// <param name="fieldInfo">
        /// KeyValuePair containing name of field and field type for the field to be added to the schema
        /// </param>
        public void AddField(KeyValuePair<string,TFieldType> fieldInfo)
        {
            AddField(fieldInfo.Key, fieldInfo.Value);
        }

        /// <summary>
        /// Add a new field to the record schema.
        /// If the field name is already in the schema, an exception will be raised.
        /// </summary>
        /// <param name="fieldInfo">
        /// IFieldNameValuePair containing name of field and field type for the field to be added to the schema
        /// </param>
        public void AddField(IFieldNameValuePair<TFieldType> fieldInfo)
        {
            AddField(fieldInfo.Name, fieldInfo.Value);
        }

    } // /class
} // /namespace
