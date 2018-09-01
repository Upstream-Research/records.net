/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IRecordAccessorAdapter to convert field values from a base record into string values
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TFieldType"></typeparam>
    public class PrintingRecordAccessor<TValue,TFieldType>
    : IRecordAccessorAdapter<string, IRecordAccessor<TValue>>
    where TFieldType: IRecordFieldType<TValue>
    {
        private readonly IRecordSchemaAccessor<TFieldType> _recordSchema;
        private readonly CultureInfo _stringCulture;
        private IRecordAccessor<TValue> _baseRecord;

        /// <summary>
        /// Create a new record accessor that will expose the values
        /// of an underlying record as strings
        /// </summary>
        /// <param name="recordSchema">field schema of the underlying record type</param>
        /// <param name="stringCulture">culture associated with the string representation</param>
        public PrintingRecordAccessor(
             IRecordSchemaAccessor<TFieldType> recordSchema
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

        private IRecordSchemaAccessor<TFieldType> RecordSchema
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
            TFieldType fieldType;

            stringValue = null;
            if (null != BaseRecord
                && BaseRecord.TryGetValue(fieldName, out fieldValue)
                )
            {
                RecordSchema.TryGetValue(fieldName, out fieldType);
                stringValue = PrintFieldValue(fieldValue, fieldType);
                hasValue = true;
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
        /// Get an enumerator of field names
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
        /// Get an enumerator of field names and field values as strings
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            if (null != BaseRecord)
            {
                IEnumerator<KeyValuePair<string,TValue>> baseEnumerator = BaseRecord.GetEnumerator();
                while (baseEnumerator.MoveNext())
                {
                    string fieldName = baseEnumerator.Current.Key;
                    TValue fieldValue = baseEnumerator.Current.Value;
                    TFieldType fieldType = RecordSchema[fieldName];
                    string stringValue = PrintFieldValue(fieldValue, fieldType);

                    yield return new KeyValuePair<string,string>(fieldName, stringValue);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace
