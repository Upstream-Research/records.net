/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Upstream.System.Records.Data
{
    /// <summary>
    /// Implements IRecordAccessor for a .NET IDataRecord
    /// </summary>
    public class DbDataRecordAccessor
    : IRecordAccessorAdapter<object, IDataRecord>
    {
        private IDataRecord _baseRecord;

        /// <summary>
        /// Create a record accessor that is not attached to an IDataRecord
        /// </summary>
        public DbDataRecordAccessor()
        {
        }

        /// <summary>
        /// Create a record accessor that will attach to an IDataRecord
        /// </summary>
        /// <param name="baseRecord"></param>
        public DbDataRecordAccessor(
            IDataRecord baseRecord
            )
        {
            AttachTo(baseRecord);
        }

        /// <summary>
        /// Get the IDataRecord to which this accessor is attached,
        /// returns a null reference if no data record is attached.
        /// </summary>
        public IDataRecord DataRecord
        {
            get
            {
                return _baseRecord;
            }
        }

        /// <summary>
        /// Get a field value by its position (ordinal)
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public object this[int fieldPosition]
        {
            get
            {
                IDataRecord record = GetDataRecord();
                object fieldValue = GetFieldValue(record, fieldPosition);

                return fieldValue;
            }

            set
            {
                throw new InvalidOperationException("Setting IDataRecord field is not permitted");
            }
        }

        /// <summary>
        /// Get the value of a field in the base record by the field's name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public object this[string fieldName]
        {
            get
            {
                object fieldValue;

                TryGetValue(fieldName, out fieldValue);

                return fieldValue;
            }

            set
            {
                throw new InvalidOperationException("Setting IDataRecord field is not permitted");
            }
        }

        /// <summary>
        /// Get the base IDataRecord,
        /// raise an exception if no base record is attached.
        /// </summary>
        /// <returns></returns>
        private IDataRecord GetDataRecord()
        {
            if (null == _baseRecord)
            {
                throw new InvalidOperationException("Record accessor adapter is not attached to a base record");
            }

            return _baseRecord;
        }

        /// <summary>
        /// Attach this adapter to an IDataRecord object
        /// </summary>
        /// <param name="baseRecord"></param>
        public void AttachTo(IDataRecord baseRecord)
        {
            _baseRecord = baseRecord;
        }

        /// <summary>
        /// Get the number of fields in the base record
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            IDataRecord record = GetDataRecord();

            return record.FieldCount;
        }

        /// <summary>
        /// Try to get the value of a field from the base record by name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out object fieldValue)
        {
            IDataRecord record = DataRecord;
            bool hasValue = false;

            fieldValue = null;
            if (null != record)
            {
                int fieldPosition = record.GetOrdinal(fieldName);
                if (0 <= fieldPosition)
                {
                    fieldValue = GetFieldValue(record, fieldPosition);
                    hasValue = true;
                }
            }

            return hasValue;
        }

        /// <summary>
        /// Centralized method to get a field value from the base record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        private object GetFieldValue(IDataRecord record, int fieldPosition)
        {
            object fieldValue = null;

            if (!record.IsDBNull(fieldPosition))
            {
                fieldValue = record.GetValue(fieldPosition);
            }

            return fieldValue;
        }

        /// <summary>
        /// Get an enumerator of field names from the base record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            IDataRecord record = DataRecord;
            
            if (null != record)
            {
                int fieldPosition = 0;
                while (fieldPosition < record.FieldCount)
                {
                    string fieldName = record.GetName(fieldPosition);

                    yield return fieldName;

                    fieldPosition += 1;
                }
            }
        }

        /// <summary>
        /// Get an enumerator of field names and their values
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            IDataRecord record = DataRecord;
            
            if (null != record)
            {
                int fieldPosition = 0;
                while (fieldPosition < record.FieldCount)
                {
                    string fieldName = record.GetName(fieldPosition);
                    object fieldValue = GetFieldValue(record, fieldPosition);

                    yield return new KeyValuePair<string,object>(fieldName, fieldValue);

                    fieldPosition += 1;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    } // /class

} // /namespace
