/*  Copyright (c) Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Upstream.System.Records;

namespace Upstream.System.Records.DataSets
{
    /// <summary>
    /// Implements IRecordListVisitor on a generic IList of DataRows
    /// </summary>
    public class DataRowRecordListVisitor
    : IRecordListVisitor<object>
    {
        // [20170324 ORIGIN DataRowCollectionRecordListVisitor]

        private IList<DataRow> _rowCollection;
        private int _rowPosition;
        private DataRowRecordAccessor _rowAccessor;

        /// <summary>
        /// Create a new RecordListVisitor that will be attached to specific DataTable
        /// </summary>
        /// <param name="rowCollection"></param>
        public DataRowRecordListVisitor(
            IList<DataRow> rowCollection
            )
        {
            _rowCollection = rowCollection;
            _rowPosition = -1;
            _rowAccessor = new DataRowRecordAccessor();
        }

        /// <summary>
        /// Dispose of the RecordListVisitor
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose of the RecordListVisitor
        /// </summary>
        /// <param name="notFinalizing"></param>
        protected virtual void Dispose(bool notFinalizing)
        {
            if (null == _rowCollection)
            {
                _rowCollection = null;
            }
        }


        /// <summary>
        /// Get the table that will receive new records
        /// </summary>
        public IList<DataRow> Rows
        {
            get
            {
                return _rowCollection;
            }
        }

        private IList<DataRow> GetRows()
        {
            if (null == _rowCollection)
            {
                throw new InvalidOperationException("Collection Builder is not attached to a row collection");
            }

            return _rowCollection;
        }

        /// <summary>
        /// Get the current buffer record
        /// </summary>
        public IRecordAccessor<object> Current
        {
            get
            {
                if (0 > _rowPosition)
                {
                    throw new InvalidOperationException("Enumerator is not positioned at a row");
                }

                return _rowAccessor;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return (object)Current;
            }
        }

        /// <summary>
        /// Reset the enumerator to a point before the first record
        /// </summary>
        public void Reset()
        {
            _rowPosition = -1;
            _rowAccessor.AttachTo(null);
        }

        /// <summary>
        /// Try to move to the next record in the table
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            bool hasMoved = false;
            IList<DataRow> rowCollection = Rows;

            if (null != rowCollection)
            {
                if (0 > _rowPosition)
                {
                    _rowPosition = 0;
                }
                else if (_rowPosition < rowCollection.Count)
                {
                    _rowPosition += 1;
                }
                if (_rowPosition < rowCollection.Count)
                {
                    DataRow row = (DataRow)rowCollection[_rowPosition];
                    _rowAccessor.AttachTo(row);
                    hasMoved = true;
                }
            }

            return hasMoved;
        }

        /// <summary>
        /// Try to move to a record in the list at a specific position
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        public bool MoveTo(int recordPosition)
        {
            bool hasMoved = false;
            IList<DataRow> rowCollection = Rows;

            if (null != rowCollection)
            {
                if (0 <= recordPosition
                    && recordPosition < rowCollection.Count
                    )
                {
                    DataRow row = (DataRow)rowCollection[recordPosition];
                    _rowAccessor.AttachTo(row);
                    _rowPosition = recordPosition;
                    hasMoved = true;
                }
            }

            return hasMoved;
        }

        /// <summary>
        /// Initialize the current record to its default field values
        /// </summary>
        public void InitializeCurrentItem()
        {
            DataRow row = _rowAccessor.DataRow;

            if (null != row)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    row[column] = column.DefaultValue;
                }
            }
        }

    } // /class

} // /namespace
