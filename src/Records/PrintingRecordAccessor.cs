/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Upstream.System.Records
{
    /// <summary>
    /// Adapts from an object record accessor to a string record accessor
    /// by formatting (aka. "printing") field values from an underlying record as strings
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TFieldType"></typeparam>
    public class PrintingRecordAccessor<TValue,TFieldType>
    : IRecordAccessorAdapter<string, IRecordAccessor<TValue>>
    where TFieldType: IRecordFieldType<TValue>
    {
        private readonly IRecordSchemaViewer<TFieldType> _recordSchema;
        private readonly CultureInfo _stringCulture;
        private IRecordAccessor<TValue> _baseRecord;

        /// <summary>
        /// Create a new record accessor that will expose the values
        /// of an underlying record as strings
        /// </summary>
        /// <param name="recordSchema">field schema of the underlying record type</param>
        /// <param name="stringCulture">culture associated with the string representation</param>
        public PrintingRecordAccessor(
             IRecordSchemaViewer<TFieldType> recordSchema
            ,CultureInfo stringCulture
            )
        {
            if (null == recordSchema)
            {
                throw new ArgumentNullException("recordSchema");
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

        private IRecordAccessor<TValue> BaseRecord
        {   
            get
            {
                return _baseRecord;
            }
        }

        /// <summary>
        /// Get/Set a field value from its field position
        /// </summary>
        /// <param name="fieldOrdinal"></param>
        /// <returns></returns>
        public string this[int fieldOrdinal]
        {
            get
            {
                string stringValue = null;

                if (null != BaseRecord)
                {
                    object fieldValue = BaseRecord[fieldOrdinal];
                    TFieldType fieldType = RecordSchema[fieldOrdinal];
                    stringValue = PrintFieldValue(fieldValue, fieldType);
                }

                return stringValue;
            }

            set
            {
                string stringValue = value;

                if (null != BaseRecord)
                {
                    TFieldType fieldType = RecordSchema[fieldOrdinal];
                    TValue fieldValue = ParseFieldValue(stringValue, fieldType);
                    BaseRecord[fieldOrdinal] = fieldValue;
                }
            }
        }

        /// <summary>
        /// Get/Set a field value from its field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string this[string fieldName]
        {
            get
            {
                string stringValue = null;

                if (null != BaseRecord)
                {
                    object fieldValue = BaseRecord[fieldName];
                    TFieldType fieldType = RecordSchema[fieldName];
                    stringValue = PrintFieldValue(fieldValue, fieldType);
                }

                return stringValue;
            }

            set
            {
                string stringValue = value;

                if (null != BaseRecord)
                {
                    TFieldType fieldType = RecordSchema[fieldName];
                    TValue fieldValue = ParseFieldValue(stringValue, fieldType);
                    BaseRecord[fieldName] = fieldValue;
                }
            }
        }

        /// <summary>
        /// try to get a field value from the underlying record and then try to convert it
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out string stringValue)
        {
            bool hasValue = false;
            TValue fieldValue;
            TFieldType fieldType = default(TFieldType);

            stringValue = null;
            if (null != BaseRecord
                && BaseRecord.TryGetValue(fieldName, out fieldValue)
                )
            {
                int fieldPosition = RecordSchema.IndexOfField(fieldName);
                if (0 <= fieldPosition)
                {
                    fieldType = RecordSchema[fieldPosition];
                }
                stringValue = PrintFieldValue(fieldValue, fieldType);
                hasValue = true;
            }

            return hasValue;
        }

        /// <summary>
        /// Try to find a field and its value by the field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>null reference if the field is not found.</returns>
        public IFieldNameValuePair<string> FindField(string fieldName)
        {
            IFieldNameValuePair<string> fieldItem = null;
            string fieldValue;

            if (TryGetValue(fieldName, out fieldValue))
            {
                fieldItem = new FieldNameValuePair<string>(fieldName, fieldValue);
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
            // [20170322 [db] ORIGIN ParsingRecordAccessor.cs]
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
        private TValue ParseFieldValue(string stringValue, TFieldType fieldType)
        {
            // [20170322 [db] ORIGIN ParsingRecordAccessor.cs]
            TValue fieldValue = default(TValue);

            if (null != stringValue)
            {
                if (null != fieldType)
                {
                    Type dataType = fieldType.DataType;
                    object fieldValueObject = Convert.ChangeType(stringValue, dataType, StringCulture);
                    if (null != fieldValueObject)
                    {
                        fieldValue = (TValue)fieldValueObject;
                    }
                }
            }

            return fieldValue;
        }

        /// <summary>
        /// Set the base record
        /// </summary>
        /// <param name="baseRecord"></param>
        public void AttachTo(IRecordAccessor<TValue> baseRecord)
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
        /// Get an enumerator of field names and field values as strings
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<string>> GetEnumerator()
        {
            if (null != BaseRecord)
            {
                IEnumerator<IFieldNameValuePair<TValue>> baseEnumerator = BaseRecord.GetEnumerator();
                while (baseEnumerator.MoveNext())
                {
                    string fieldName = baseEnumerator.Current.Name;
                    TValue fieldValue = baseEnumerator.Current.Value;
                    TFieldType fieldType = RecordSchema[fieldName];
                    string stringValue = PrintFieldValue(fieldValue, fieldType);

                    yield return new FieldNameValuePair<string>(fieldName, stringValue);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace
