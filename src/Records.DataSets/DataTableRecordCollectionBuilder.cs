/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Data;

using Upstream.System.Records;

namespace Upstream.System.Records.DataSets
{
    /// <summary>
    /// Implements IRecordCollectionBuilder on a DataTable
    /// </summary>
    public class DataTableRecordCollectionBuilder
    : IRecordCollectionBuilder<object>
    {
        private DataTable _table;
        private DataRowRecordAccessor _rowAccessor;

        /// <summary>
        /// Create a collection builder that is attached to a specific DataTable
        /// </summary>
        /// <param name="table"></param>
        public DataTableRecordCollectionBuilder(
            DataTable table
            )
        {
            _table = table;
            _rowAccessor = new DataRowRecordAccessor();
        }

        /// <summary>
        /// Dispose of the collection builder
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose of the collection builder
        /// </summary>
        /// <param name="notFinalizing"></param>
        protected virtual void Dispose(bool notFinalizing)
        {
            if (null != _table)
            {
                if (notFinalizing)
                {
                    _table.AcceptChanges();
                }
                _table = null;
            }
        }

        /// <summary>
        /// Get the table that will receive new records
        /// </summary>
        public DataTable DataTable
        {
            get
            {
                return _table;
            }
        }

        private DataTable GetDataTable()
        {
            if (null == _table)
            {
                throw new InvalidOperationException("Collection Builder is not attached to a DataTable");
            }

            return _table;
        }

        /// <summary>
        /// Get the current buffer record
        /// </summary>
        public IRecordAccessor<object> Current
        {
            get
            {
                DataTable table = GetDataTable();
                EnsureCurrentRow(table);

                return _rowAccessor;
            }
        }

        /// <summary>
        /// Ensure that the current row is ready for modifications
        /// </summary>
        /// <returns></returns>
        private DataRow EnsureCurrentRow(DataTable table)
        {
            DataRow row = _rowAccessor.DataRow;

            if (null == row && null != table)
            {
                row = table.NewRow();
                _rowAccessor.AttachTo(row);
            }

            return row;
        }

        /// <summary>
        /// Initialize the current record to default values
        /// </summary>
        public void InitializeCurrentItem()
        {
            DataTable table = GetDataTable();
            DataRow row = EnsureCurrentRow(table);

            foreach (DataColumn column in table.Columns)
            {
                row[column] = column.DefaultValue;
            }
        }

        /// <summary>
        /// Add the current row item to the table
        /// </summary>
        public bool AddCurrentItem()
        {
            DataTable table = GetDataTable();
            DataRow row = EnsureCurrentRow(table);

            table.Rows.Add(row);

            return true;
        }

        /// <summary>
        /// Copy the values from a record into a new row in the table
        /// </summary>
        /// <param name="record"></param>
        public bool Add(IRecordAccessor<object> record)
        {
            InitializeCurrentItem();
            RecordIO.CopyInto(Current, record);
            return AddCurrentItem();
        }

    } // /class

} // /namespace
