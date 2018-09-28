/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

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
        private readonly IRecordSchemaViewer<TFieldType> _recordSchema;
        private readonly CultureInfo _stringCulture;
        private IRecordAccessor<string> _baseRecord;

        /// <summary>
        /// Create a record accessor that will parse strings
        /// according to the field types provided.
        /// </summary>
        /// <param name="recordSchema">
        /// record schema for the parsed records.
        /// </param>
        /// <param name="stringCulture">
        /// culture associated with the base record string representation
        /// </param>
        public ParsingRecordAccessor(
            IRecordSchemaViewer<TFieldType> recordSchema
            ,CultureInfo stringCulture
            )
        {
            if (null == recordSchema)
            {
                throw new ArgumentNullException("recordSchema");
            }
            if (null == stringCulture)
            {
                throw new ArgumentNullException("stringCulture");
            }

            _recordSchema = recordSchema;
            _stringCulture = stringCulture;
        }

        private IRecordSchemaViewer<TFieldType> RecordSchema
        {
            get
            {
                return _recordSchema;
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
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public object this[int fieldPosition]
        {
            get
            {
                TFieldType fieldType = RecordSchema[fieldPosition];
                string stringValue = null;

                if (null != BaseRecord)
                {
                    stringValue = BaseRecord[fieldPosition];
                }
                
                return ParseFieldValue(stringValue, fieldType);
            }

            set
            {
                TFieldType fieldType = RecordSchema[fieldPosition];
                object fieldValue = value;
                if (null != BaseRecord)
                {
                    string stringValue = PrintFieldValue(fieldValue, fieldType);
                    BaseRecord[fieldPosition] = stringValue;
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
                TFieldType fieldType = RecordSchema[fieldName];
                string stringValue = null;

                if (null != BaseRecord)
                {
                    stringValue = BaseRecord[fieldName];
                }
                
                return ParseFieldValue(stringValue, fieldType);
            }

            set
            {
                TFieldType fieldType = RecordSchema[fieldName];
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
            TFieldType fieldType = default(TFieldType);

            fieldValue = null;
            if (null != BaseRecord
                && BaseRecord.TryGetValue(fieldName, out stringValue)
                )
            {
                int fieldPosition = RecordSchema.IndexOfField(fieldName);
                if (0 <= fieldPosition)
                {
                    fieldType = RecordSchema[fieldPosition];
                }
                fieldValue = ParseFieldValue(stringValue, fieldType);
                hasValue = true;
            }

            return hasValue;
        }

        /// <summary>
        /// Try to find a field and its value by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>
        /// null reference if the field is not found.
        /// </returns>
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
            return RecordSchema.IndexOfField(fieldName);
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
                    Type dataType = fieldType.SystemType;
                    try
                    {
                        fieldValue = Convert.ChangeType(stringValue, dataType, StringCulture);
                    }
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
        /// Get an enumerator of field names and field values
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<object>> GetEnumerator()
        {
            if (null != BaseRecord)
            {
                IEnumerator<IFieldNameValuePair<string>> baseEnumerator = BaseRecord.GetEnumerator();
                while (baseEnumerator.MoveNext())
                {
                    string fieldName = baseEnumerator.Current.Name;
                    string stringValue = baseEnumerator.Current.Value;
                    TFieldType fieldType = RecordSchema[fieldName];
                    object fieldValue = ParseFieldValue(stringValue, fieldType);

                    yield return new FieldNameValuePair<object>(fieldName, fieldValue);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace
