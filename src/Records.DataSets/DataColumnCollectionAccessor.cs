/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using Upstream.System.Records;

namespace Upstream.System.Records.DataSets
{
    /// <summary>
    /// Implements IRecordAccessor on a DataColumnCollection
    /// </summary>
    public class DataColumnCollectionAccessor
    : IRecordAccessorAdapter<DataColumnFieldType, DataColumnCollection>
     ,IRecordSchemaAccessor<DataColumnFieldType>
    {
        private DataColumnCollection _columnCollection;
        private DataColumnNameEnumeration _columnNameEnumeration;
        private IList<DataColumnFieldType> _columnFieldTypeList = new List<DataColumnFieldType>();
        private IDictionary<string,DataColumnFieldType> _columnFieldTypeDictionary = new Dictionary<string,DataColumnFieldType>();

        /// <summary>
        /// Create a new DataColumnCollectionAccessor that is not attached to any column collection
        /// </summary>
        public DataColumnCollectionAccessor()
        {
        }

        /// <summary>
        /// Create a new DataColumnCollectionAccessor from a base DataColumnCollection
        /// </summary>
        /// <param name="baseColumnCollection"></param>
        public DataColumnCollectionAccessor(DataColumnCollection baseColumnCollection)
        {
            AttachTo(baseColumnCollection);
        }

        public IEnumerable<string> FieldNames
        {
            get
            {
                return _columnNameEnumeration;
            }
        }

        /// <summary>
        /// Attach to a base DataColumnCollection
        /// </summary>
        /// <param name="baseColumnCollection"></param>
        public void AttachTo(DataColumnCollection baseColumnCollection)
        {
            if (null != _columnCollection)
            {
                _columnCollection.CollectionChanged -= _columnCollection_CollectionChanged;
                _columnCollection = null;
                _columnNameEnumeration = null;
                _columnFieldTypeList.Clear();
                _columnFieldTypeDictionary.Clear();
            }

            _columnCollection = baseColumnCollection;
            _columnCollection.CollectionChanged += _columnCollection_CollectionChanged;
            foreach (DataColumn column in _columnCollection)
            {
                AddDataColumn(column);
            }
            _columnNameEnumeration = new DataColumnNameEnumeration(_columnCollection);
        }

        private void AddDataColumn(DataColumn column)
        {
            string fieldName = column.ColumnName;
            DataColumnFieldType fieldType = new DataColumnFieldType(column);
            _columnFieldTypeList.Add(fieldType);
            _columnFieldTypeDictionary.Add(fieldName, fieldType);
        }

        private void _columnCollection_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                DataColumn column = (DataColumn)e.Element;
                AddDataColumn(column);
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DataColumn column = (DataColumn)e.Element;
                string fieldName = column.ColumnName;
                int fieldPosition = column.Ordinal;
                _columnFieldTypeList.RemoveAt(fieldPosition);
                _columnFieldTypeDictionary.Remove(fieldName);
            }
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                // reattach
                AttachTo(_columnCollection);
            }
        }

        /// <summary>
        /// Get a column by its position
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public DataColumnFieldType this[int fieldPosition]
        {
            get
            {
                return _columnFieldTypeList[fieldPosition];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get a field type by field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public DataColumnFieldType this[string fieldName]
        {
            get
            {
                return _columnFieldTypeDictionary[fieldName];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the number of columns
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return _columnFieldTypeDictionary.Count;
        }

        /// <summary>
        /// Try to get a field type by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out DataColumnFieldType outValue)
        {
            return _columnFieldTypeDictionary.TryGetValue(fieldName, out outValue);
        }

        /// <summary>
        /// Get an enumerator of field names and types
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, DataColumnFieldType>> GetEnumerator()
        {
            return _columnFieldTypeDictionary.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator of field names for this record
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetFieldNameEnumerator()
        {
            foreach (DataColumnFieldType columnFieldType in _columnFieldTypeList)
            {
                DataColumn column = columnFieldType.DataColumn;
                string fieldName = column.ColumnName;

                yield return fieldName;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        /// <summary>
        /// Find the position of a field in the column collection by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int IndexOfField(string fieldName)
        {
            int fieldPosition = -1;

            if (null != _columnCollection)
            {
                fieldPosition = _columnCollection.IndexOf(fieldName);
            }
            
            return fieldPosition;
        }

        /// <summary>
        /// Find the name of a field at a specific location
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public string FieldNameAt(int fieldPosition)
        {
            string fieldName = null;

            if (null == _columnCollection)
            {
                throw new InvalidOperationException("DataColumnCollectionAccessor is not attached to a DataColumnCollection");
            }

            DataColumn column = _columnCollection[fieldPosition];
            if (null != column)
            {
                fieldName = column.ColumnName;
            }

            return fieldName;
        }

        /// <summary>
        /// Implements an IEnumerator of DataColumn ColumnName values
        /// </summary>
        private class DataColumnNameEnumerator
        : IEnumerator<string>
        {
            private readonly IEnumerator _dataColumnEnumerator;

            public DataColumnNameEnumerator(
                IEnumerator dataColumnEnumerator
                )
            {
                _dataColumnEnumerator = dataColumnEnumerator;
            }

            public string Current
            {
                get
                {
                    DataColumn column = (DataColumn)_dataColumnEnumerator.Current;
                    string columnName = null;

                    if (null != column)
                    {
                        columnName = column.ColumnName;
                    }

                    return columnName;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                // do nothing;
            }

            public bool MoveNext()
            {
                return _dataColumnEnumerator.MoveNext();
            }

            public void Reset()
            {
                _dataColumnEnumerator.Reset();
            }

        } // /class

        /// <summary>
        /// Implements an IEnumerable of DataColumn ColumnName values
        /// </summary>
        private class DataColumnNameEnumeration
        : IEnumerable<string>
        {
            private readonly IEnumerable _dataColumnEnumeration;

            public DataColumnNameEnumeration(
                DataColumnCollection dataColumnCollection
                )
            {
                _dataColumnEnumeration = dataColumnCollection;
            }

            public IEnumerator<string> GetEnumerator()
            {
                return new DataColumnNameEnumerator(_dataColumnEnumeration.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        } // /class

    } // /class

} // /namespace
