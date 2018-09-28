/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides access to meta-fields for a field parsed from a Field Schema Spec string.
    /// </summary>
    public class FieldSchemaSpecFieldType<TValue>
    : IRecordViewer<object>
     , IRecordFieldType<TValue>
    {
        /// <summary>
        /// A "meta schema" that describes the field type attributes on a FieldSchemaSpecFieldType.
        /// </summary>
        private static IRecordSchemaViewer<IRecordFieldType<object>> MetaSchema
         = new BasicRecordSchema<IRecordFieldType<object>>(
            new FieldNameValuePairEnumeration<IRecordFieldType<object>>(
                new string[] {
                     "type_name"
                }
                ,new IRecordFieldType<object>[] {
                    new BasicRecordFieldType<object>(typeof(string))
                }
            )
        );
        
        private const int FieldTypeNamePosition = 0;

        private readonly IRecordAccessor<object> _metaFields;
        private BasicRecordFieldType<TValue>  _baseFieldType;

        /// <summary>
        /// Create a new field description record for a field parsed from a field schema spec string
        /// </summary>
        /// <param name="fieldTypeName"></param>
        /// <param name="systemType"></param>
        internal FieldSchemaSpecFieldType(
             string fieldTypeName
            ,Type systemType
            )
        {
            IList<object> metaValueList = new object[] {
                fieldTypeName
            };

            _baseFieldType = new BasicRecordFieldType<TValue>(systemType);
            _metaFields = new ListRecordAccessor<object,IRecordFieldType<object>>(MetaSchema, metaValueList);
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
                return _metaFields[fieldPosition];
            }

            set
            {
                _metaFields[fieldPosition] = value;
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
                return _metaFields[fieldName];
            }

            set
            {
                _metaFields[fieldName] = value;
            }
        }

        /// <summary>
        /// Get the name of the field type as it was provided in the field schema spec
        /// </summary>
        public string FieldTypeName
        {
            get
            {
                return (string)_metaFields[FieldTypeNamePosition];
            }
        }

        /// <summary>
        /// Get the .NET datatype for field values belonging to the field described by this record
        /// </summary>
        public Type SystemType
        {
            get
            {
                return _baseFieldType.SystemType;
            }
        }

        /// <summary>
        /// Get the number of meta fields in this record
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return _metaFields.GetFieldCount();
        }
        
        /// <summary>
        /// Try to get the value of a metafield by name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out object fieldValue)
        {
            return _metaFields.TryGetValue(fieldName, out fieldValue);
        }

        /// <summary>
        /// Try to find a field and its value by the field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>null reference if the field is not found</returns>
        public IFieldNameValuePair<object> FindField(string fieldName)
        {
            IFieldNameValuePair<object> fieldItem = null;
            object fieldValue;

            if (TryGetValue(fieldName, out fieldValue))
            {
                fieldItem = new FieldNameValuePair<object>(fieldName, fieldValue);
            }

            return fieldItem;
        }

        /// <summary>
        /// Try to get a field's position by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>negative if field name was not found</returns>
        public int IndexOfField(string fieldName)
        {
            return _metaFields.IndexOfField(fieldName);
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
        /// Get an enumerator of the metafields for this field
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<object>> GetEnumerator()
        {
            return _metaFields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace

