/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Upstream.System.Records;
using Upstream.System.Records.Data;

namespace Upstream.System.Records.DataSets
{
    /// <summary>
    /// Implements IRecordList over an attached ADO.Net DataTable
    /// </summary>
    public class DataTableRecordList
    : IRecordList<object, DataColumnFieldType>
     ,IDbDataSink
    {
        private DataTable _table;
        private DataColumnCollectionAccessor _fieldSchema = new DataColumnCollectionAccessor();

        /// <summary>
        /// Create a new RecordList that is attached to a specific DataTable
        /// </summary>
        /// <param name="table"></param>
        public DataTableRecordList(DataTable table)
        {
            AttachTo(table);
        }

        /// <summary>
        /// Attach this record list to a base DataTable
        /// </summary>
        /// <param name="table"></param>
        public void AttachTo(DataTable table)
        {
            _table = table;
            if (null == _table)
            {
                _fieldSchema.AttachTo(null);
            }
            else
            {
                _fieldSchema.AttachTo(table.Columns);
            }
        }

        /// <summary>
        /// Get the attached DataTable, or a null reference if no DataTable is attached.
        /// </summary>
        public DataTable DataTable
        {
            get
            {
                return _table;
            }
        }

        /// <summary>
        /// Get the attached DataTable, raise an exception if no table is attached
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataTable()
        {
            if (null == _table)
            {
                throw new InvalidOperationException("DataTableRecordList is not attached to a DataTable");
            }

            return _table;
        }

        /// <summary>
        /// Get the number of records in the collection
        /// </summary>
        public int Count
        {
            get
            {
                DataTable table = GetDataTable();

                return table.Rows.Count;
            }
        }

        /// <summary>
        /// Get the number of fields in each record in this collection
        /// </summary>
        public int FieldCount
        {
            get
            {
                return FieldSchema.GetFieldCount();
            }
        }

        /// <summary>
        /// Get an object that can enumerate the field names for records in this collection
        /// </summary>
        public IEnumerable<string> FieldNames
        {
            get
            {
                DataTable table = GetDataTable();

                return new DataColumnNameCollection(table);
            }
        }

        /// <summary>
        /// Get an object that describes the fields in this record collection
        /// </summary>
        public IRecordAccessor<DataColumnFieldType> FieldSchema
        {
            get
            {
                return _fieldSchema;
            }
        }

        /// <summary>
        /// Get an accessor to a record at a position.
        /// This is not an efficient way to loop over a record collection;
        /// use an enumerator or visitor to do that.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        public IRecordAccessor<object> this[int recordPosition]
        {
            get
            {
                DataTable table = GetDataTable();
                DataRow row = table.Rows[recordPosition];
                IRecordAccessor<object> rowAccessor = CreateDataRowRecordAccessor(row);

                return rowAccessor;
            }

            set
            {
                IRecordAccessor<object> record = value;
                DataTable table = GetDataTable();
                DataRow row = table.Rows[recordPosition];
                CopyRecordInto(row, record);
            }
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private DataRowRecordAccessor 
        CreateDataRowRecordAccessor(
            DataRow row
            )
        {
            DataRowRecordAccessor rowAccessor = new DataRowRecordAccessor(row);

            return rowAccessor;
        }

        /// <summary>
        /// Centralized method
        /// </summary>
        /// <param name="targetRow"></param>
        /// <param name="record"></param>
        private void 
        CopyRecordInto(
             DataRow targetRow
            ,IRecordAccessor<object> record
            )
        {
            IRecordAccessor<object> targetRecord = CreateDataRowRecordAccessor(targetRow);
            IEnumerator<string> fieldNameEnumerator = targetRecord.GetFieldNameEnumerator();
            while (fieldNameEnumerator.MoveNext())
            {
                string fieldName = fieldNameEnumerator.Current;
                object fieldValue;
                if (record.TryGetValue(fieldName, out fieldValue))
                {
                    targetRecord[fieldName] = fieldValue;
                }
            }
        }

        /// <summary>
        /// Remove all records from the collection
        /// </summary>
        public void Clear()
        {
            DataTable table = DataTable;

            if (null != table)
            {
                table.Rows.Clear();
            }
        }

        /// <summary>
        /// Add a new record to the end of the collection
        /// </summary>
        /// <param name="record"></param>
        public void Add(IRecordAccessor<object> record)
        {
            int insertPosition = -1;

            Insert(insertPosition, record);
        }

        /// <summary>
        /// Insert the values from a record as a new row in the table
        /// </summary>
        /// <param name="insertPosition"></param>
        /// <param name="record"></param>
        public void 
        Insert(
             int insertPosition
            ,IRecordAccessor<object> record
            )
        {
            DataTable table = GetDataTable();

            if (null != record)
            {
                DataRow row = table.NewRow();
                CopyRecordInto(row, record);
                if (0 <= insertPosition
                    && insertPosition < table.Rows.Count
                    )
                {
                    table.Rows.InsertAt(row, insertPosition);
                }
                else
                {
                    table.Rows.Add(row);
                }
            }
        }

        /// <summary>
        /// Remove a record at a specified position
        /// </summary>
        /// <param name="recordPosition"></param>
        public void RemoveAt(int recordPosition)
        {
            DataTable table = GetDataTable();

            table.Rows.RemoveAt(recordPosition);
        }

        /// <summary>
        /// Get a writer object that can add new records to the table
        /// </summary>
        /// <returns></returns>
        public IRecordCollectionBuilder<object> 
        GetRecordCollectionBuilder()
        {
            DataTable table = GetDataTable();

            return new DataTableRecordCollectionBuilder(table);
        }

        /// <summary>
        /// Get a random-access table row record cursor
        /// </summary>
        /// <returns></returns>
        public IRecordListVisitor<object> GetRecordListVisitor()
        {
            DataTable table = GetDataTable();

            return new DataRowCollectionRecordListVisitor(table.Rows);
        }

        /// <summary>
        /// Get an IRecordEnumerator of table row records
        /// </summary>
        /// <returns></returns>
        public IRecordEnumerator<object> GetRecordEnumerator()
        {
            return GetRecordListVisitor();
        }

        /// <summary>
        /// Get a generic enumerator of table row records
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IRecordAccessor<object>> GetEnumerator()
        {
            return GetRecordEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        /// <summary>
        /// Read new rows into the table using an IDataReader
        /// </summary>
        /// <param name="reader"></param>
        public void Load(IDataReader reader)
        {
            if (null != DataTable)
            {
                DataTable.Load(reader);
            }
        }

        /// <summary>
        /// Implements IEnumerable for the column names in a DataTable
        /// </summary>
        private class DataColumnNameCollection
        : IEnumerable<string>
        {
            private readonly DataTable _table;

            internal DataColumnNameCollection(
                DataTable table
                )
            {
                _table = table;
            }

            /// <summary>
            /// Get an enumerator of column names
            /// </summary>
            /// <returns></returns>
            public IEnumerator<string> GetEnumerator()
            {
                foreach (DataColumn column in _table.Columns)
                {
                    string columnName = column.ColumnName;

                    yield return columnName;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }

        } // /class

    } // /class

} // /namespace

