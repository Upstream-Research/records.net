/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Upstream.System.Records
{
    /// <summary>
    /// Adapts from a string record accessor to an object record accessor
    /// by parsing datatypes according to Convert.ChangeType() (which uses IConvertible)
    /// </summary>
    public class ParsingRecordAccessor<TFieldType>
    : IRecordAccessorAdapter<object, IRecordAccessor<string>>
    where TFieldType : IRecordFieldType<object>
    {
        private readonly IRecordAccessor<TFieldType> _fieldSchema;
        private readonly CultureInfo _stringCulture;
        private IRecordAccessor<string> _baseRecord;

        /// <summary>
        /// Create a record accessor that will parse strings
        /// according to the field types provided.
        /// </summary>
        /// <param name="fieldSchema"></param>
        /// <param name="stringCulture">culture associated with the base record string representation</param>
        public ParsingRecordAccessor(
            IRecordAccessor<TFieldType> fieldSchema
            ,CultureInfo stringCulture
            )
        {
            if (null == fieldSchema)
            {
                throw new ArgumentNullException("fieldSchema");
            }
            if (null == stringCulture)
            {
                throw new ArgumentNullException("stringCulture");
            }

            _fieldSchema = fieldSchema;
            _stringCulture = stringCulture;
        }

        private IRecordAccessor<TFieldType> FieldSchema
        {
            get
            {
                return _fieldSchema;
            }
        }

        private CultureInfo StringCulture
        {
            get
            {
                return _stringCulture;
            }
        }

        private IRecordAccessor<string> BaseRecord
        {   
            get
            {
                return _baseRecord;
            }
        }

        /// <summary>
        /// Get/Set a field value by its field position
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        public object this[int fieldOrdinal]
        {
            get
            {
                TFieldType fieldType = FieldSchema[fieldOrdinal];
                string stringValue = null;

                if (null != BaseRecord)
                {
                    stringValue = BaseRecord[fieldOrdinal];
                }
                
                return ParseFieldValue(stringValue, fieldType);
            }

            set
            {
                TFieldType fieldType = FieldSchema[fieldOrdinal];
                object fieldValue = value;
                if (null != BaseRecord)
                {
                    string stringValue = PrintFieldValue(fieldValue, fieldType);
                    BaseRecord[fieldOrdinal] = stringValue;
                }
            }
        }

        /// <summary>
        /// Get/Set a field value by its field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object this[string fieldName]
        {
            get
            {
                TFieldType fieldType = FieldSchema[fieldName];
                string stringValue = null;

                if (null != BaseRecord)
                {
                    stringValue = BaseRecord[fieldName];
                }
                
                return ParseFieldValue(stringValue, fieldType);
            }

            set
            {
                TFieldType fieldType = FieldSchema[fieldName];
                object fieldValue = value;

                if (null != BaseRecord)
                {
                    string stringValue = PrintFieldValue(fieldValue, fieldType);
                    BaseRecord[fieldName] = stringValue;
                }
            }
        }

        /// <summary>
        /// Try to get a value from the underlying record
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out object fieldValue)
        {
            bool hasValue = false;
            string stringValue;
            TFieldType fieldType;

            fieldValue = null;
            if (null != BaseRecord
                && BaseRecord.TryGetValue(fieldName, out stringValue)
                )
            {
                FieldSchema.TryGetValue(fieldName, out fieldType);
                try
                {
                    fieldValue = ParseFieldValue(stringValue, fieldType);
                    hasValue = true;
                }
                // These are all exceptions raised by Convert.ChangeType(),
                //  which is used by ParseFieldValue()
                catch (InvalidCastException)
                {
                    // continue;
                }
                catch (FormatException)
                {
                    // continue;
                }
                catch (OverflowException)
                {
                    // continue;
                }
            }

            return hasValue;
        }

        /// <summary>
        /// Convert a field value to a string
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        private string PrintFieldValue(object fieldValue, TFieldType fieldType)
        {
            string stringValue = fieldValue as string;

            if (null != fieldValue
                && null == stringValue
                )
            {
                stringValue = Convert.ToString(fieldValue, StringCulture);
            }

            return stringValue;
        }

        /// <summary>
        /// Parse a field value from a string
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        private object ParseFieldValue(string stringValue, TFieldType fieldType)
        {
            object fieldValue = null;

            if (null != stringValue)
            {
                if (null != fieldType)
                {
                    Type dataType = fieldType.DataType;
                    fieldValue = Convert.ChangeType(stringValue, dataType, StringCulture);
                }
                else
                {
                    fieldValue = stringValue;
                }
            }

            return fieldValue;
        }


        /// <summary>
        /// Attach this record accessor to a base record with string values
        /// </summary>
        /// <param name="baseRecord"></param>
        public void AttachTo(IRecordAccessor<string> baseRecord)
        {
            _baseRecord = baseRecord;
        }

        /// <summary>
        /// Get the number of fields in the record
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            int fieldCount = 0;

            if (null != BaseRecord)
            {
                fieldCount = BaseRecord.GetFieldCount();
            }

            return fieldCount;
        }

        /// <summary>
        /// Get an enumerator of field names from the underlying record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            IEnumerator<string> fieldNameEnumerator;

            if (null == BaseRecord)
            {
                IEnumerable<string> emptyEnumeration = new string[0];
                fieldNameEnumerator = emptyEnumeration.GetEnumerator();
            }
            else
            {
                fieldNameEnumerator = BaseRecord.GetFieldNameEnumerator();
            }

            return fieldNameEnumerator;
        }

        /// <summary>
        /// Get an enumerator of field names and field values
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            if (null != BaseRecord)
            {
                IEnumerator<KeyValuePair<string,string>> baseEnumerator = BaseRecord.GetEnumerator();
                while (baseEnumerator.MoveNext())
                {
                    string fieldName = baseEnumerator.Current.Key;
                    string stringValue = baseEnumerator.Current.Value;
                    TFieldType fieldType = FieldSchema[fieldName];
                    object fieldValue = ParseFieldValue(stringValue, fieldType);

                    yield return new KeyValuePair<string,object>(fieldName, fieldValue);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace
