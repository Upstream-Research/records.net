/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Upstream.System.Records.DataSets
{
    /// <summary>
    /// Implements IRecordAccessor for an ADO.Net DataRow
    /// </summary>
    public class DataRowRecordAccessor
    : IRecordAccessorAdapter<object, DataRow>
    {
        private DataRow _row;

        /// <summary>
        /// Create a new DataRow accessor that is not attached to a DataRow
        /// </summary>
        public DataRowRecordAccessor()
        {
        }

        /// <summary>
        /// Create a DataRow accessor that is attached to specific DataRow
        /// </summary>
        /// <param name="row"></param>
        public DataRowRecordAccessor(
            DataRow row
            )
        {
            AttachTo(row);
        }

        /// <summary>
        /// Attach to a DataRow object
        /// </summary>
        /// <param name="baseRecord"></param>
        public void AttachTo(DataRow baseRecord)
        {
            _row = baseRecord;
        }

        /// <summary>
        /// Get the currently attached DataRow, or a null reference if no row is attached
        /// </summary>
        public DataRow DataRow
        {
            get
            {
                return _row;
            }
        }

        /// <summary>
        /// Get the attached DataRow, raise an exception if no row is attached
        /// </summary>
        /// <returns></returns>
        private DataRow GetDataRow()
        {
            if (null == _row)
            {
                throw new InvalidOperationException("No DataRow is attached");
            }

            return _row;
        }

        /// <summary>
        /// Get/Set a field value by its position in the record
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public object this[int fieldPosition]
        {
            get
            {
                DataRow row = GetDataRow();
                object columnValue = row[fieldPosition];
                object fieldValue = ConvertToFieldValue(columnValue);

                return fieldValue;
            }

            set
            {
                DataRow row = GetDataRow();
                object fieldValue = value;
                object columnValue = ConvertToColumnValue(fieldValue);
                row[fieldPosition] = columnValue;
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
                DataRow row = GetDataRow();
                object columnValue = row[fieldName];
                object fieldValue = ConvertToFieldValue(columnValue);

                return fieldValue;
            }

            set
            {
                DataRow row = GetDataRow();
                object fieldValue = value;
                object columnValue = ConvertToColumnValue(fieldValue);
                row[fieldName] = columnValue;
            }
        }

        private object ConvertToFieldValue(object columnValue)
        {
            object fieldValue = columnValue;
            if (Convert.IsDBNull(fieldValue))
            {
                fieldValue = null;
            }

            return fieldValue;
        }

        private object ConvertToColumnValue(object fieldValue)
        {
            object columnValue = fieldValue;
            if (null == columnValue)
            {
                columnValue = DBNull.Value;
            }

            return columnValue;
        }

        /// <summary>
        /// Centralized helper method
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private object GetFieldValue(DataRow row, DataColumn column)
        {
            object columnValue = row[column];
            object fieldValue = ConvertToFieldValue(columnValue);

            return fieldValue;
        }

        /// <summary>
        /// Get the number of fields in the record
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return GetDataRow().Table.Columns.Count;
        }

        /// <summary>
        /// Try to get a field value from a field by name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out object fieldValue)
        {
            bool hasValue = false;
            DataRow row = DataRow;

            fieldValue = null;
            if (null != row)
            {
                DataTable table = row.Table;
                DataColumn column = table.Columns[fieldName];
                if (null != column)
                {
                    fieldValue = GetFieldValue(row, column);
                    hasValue = true;
                }
            }

            return hasValue;
        }

        /// <summary>
        /// Get an enumerator of field names for this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            DataRow row = GetDataRow();
            DataTable table = row.Table;

            foreach (DataColumn column in table.Columns)
            {
                string fieldName = column.ColumnName;

                yield return fieldName;
            }
        }

        /// <summary>
        /// Get an enumerator of field names and values for this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            DataRow row = GetDataRow();
            DataTable table = row.Table;

            foreach (DataColumn column in table.Columns)
            {
                string fieldName = column.ColumnName;
                object fieldValue = GetFieldValue(row, column);

                yield return new KeyValuePair<string,object>(fieldName,fieldValue);
            }            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // /class

} // /namespace
