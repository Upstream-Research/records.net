/*  Copyright (c) 2016 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements an IRecordCollectionIndex over an IRecordList.
    /// The index cannot detect changes to the underlying list,
    /// so if the list is changed, the index must be updated separately.
    /// </summary>
    public class RecordListIndex<TValue>
    : IRecordCollectionIndex<TValue>
    {
        private readonly IList<string> _keyFieldNameList;
        private readonly IRecordList<TValue> _baseRecordList;
        /// <summary>
        /// The index will store a list of record positions for each distinct key.
        /// </summary>
        private readonly IDictionary<TValue[],IList<int>> _recordPositionDictionary;

        /// <summary>
        /// Creates a new record index object,
        /// the actual index must be "built" by calling BuildIndex().
        /// Once the index is built, it should be considered invalid if the underlying list is changed.
        /// </summary>
        /// <param name="recordList"></param>
        /// <param name="keyFieldNameEnumeration"></param>
        public RecordListIndex(
             IRecordList<TValue> recordList
            ,IEnumerable<string> keyFieldNameEnumeration
            )
        {
            if (null == recordList)
            {
                throw new ArgumentNullException("recordList");
            }
            if (null == keyFieldNameEnumeration)
            {
                throw new ArgumentNullException("keyFieldNameEnumeration");
            }

            _keyFieldNameList = new List<string>(keyFieldNameEnumeration);
            _baseRecordList = recordList;

            int dictionaryCapacity = recordList.Count;
            IEqualityComparer<TValue[]> keyComparer = new ArrayValueEqualityComparer();
            _recordPositionDictionary = new Dictionary<TValue[],IList<int>>(
                dictionaryCapacity
                ,keyComparer
                );
        }

        /// <summary>
        /// Get an enumeration of the field names used to build this index
        /// </summary>
        public IEnumerable<string> KeyFieldNames
        {
            get
            {
                return _keyFieldNameList;
            }
        }

        /// <summary>
        /// Get the base RecordList to which this index is attached
        /// </summary>
        public IRecordList<TValue> RecordList
        {
            get
            {
                return _baseRecordList;
            }
        }

        /// <summary>
        /// Find the first record in the index that matches the fields in a key record
        /// </summary>
        /// <param name="keyRecord"></param>
        /// <returns></returns>
        public IRecordAccessor<TValue> FindFirst(IRecordAccessor<TValue> keyRecord)
        {
            IRecordAccessor<TValue> record = null;
            IList<int> recordPositionList = null;

            if (null != keyRecord)
            {
                TValue[] key = CreateFieldKey(keyRecord);
                _recordPositionDictionary.TryGetValue(key, out recordPositionList);
            }
            if (null != recordPositionList
                && 0 < recordPositionList.Count
                )
            {
                int recordPosition = recordPositionList[0];
                record = RecordList[recordPosition];
            }

            return record;
        }

        /// <summary>
        /// Get a reader that will iterate over all records found by the index
        /// </summary>
        /// <param name="keyRecord"></param>
        /// <returns></returns>
        public IRecordCollectionReader<TValue> 
        OpenRecordCollectionReader(IRecordAccessor<TValue> keyRecord)
        {
            IList<int> recordPositionList = null;

            if (null != keyRecord)
            {
                TValue[] key = CreateFieldKey(keyRecord);
                _recordPositionDictionary.TryGetValue(key, out recordPositionList);
            }
            if (null == recordPositionList)
            {
                recordPositionList = new int[0];  // empty list
            }

            IRecordListCursor<TValue> recordCursor = RecordList.OpenRecordListCursor();
            return new RecordListIndexSearchResultReader(
                 recordPositionList
                ,recordCursor
                );
        }

        /// <summary>
        /// Read the record list and build an efficient search index for the keys on this index
        /// </summary>
        public void BuildIndex()
        {
            using (IRecordListCursor<TValue> recordCursor = RecordList.OpenRecordListCursor())
            {
                int recordCount = RecordList.Count;
                int recordPosition;
                TValue[] key;
                IList<int> recordPositionList;
                IRecordAccessor<TValue> record;
                for (recordPosition = 0; recordPosition < recordCount; recordPosition++)
                {
                    record = recordCursor.Seek(recordPosition);
                    key = CreateFieldKey(record);
                    if (_recordPositionDictionary.TryGetValue(key, out recordPositionList))
                    {
                        recordPositionList.Add(recordPosition);
                    }
                    else
                    {
                        recordPositionList = new List<int>(1);
                        recordPositionList.Add(recordPosition);
                        _recordPositionDictionary.Add(key, recordPositionList);
                    }

                }
            }
        }

        private TValue[] 
        CreateFieldKey(IRecordAccessor<TValue> keyRecord)
        {
            if (null == keyRecord)
            {
                return null;
            }

            int fieldCount = _keyFieldNameList.Count;
            TValue[] fieldValueArray = new TValue[fieldCount];
            int fieldOrdinal;
            
            for (fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
            {
                string fieldName = _keyFieldNameList[fieldOrdinal];
                TValue fieldValue;
                keyRecord.TryGetValue(fieldName, out fieldValue);
                fieldValueArray[fieldOrdinal] = fieldValue;
            }

            return fieldValueArray;
        }

        /// <summary>
        /// Implements an EqualityComparer on arrays that compare the arrays for
        /// "structural equality" (i.e. two arrays are equal if they have the equal values).
        /// </summary>
        /// <remarks>
        /// This does not use IStructuralEquatable (.NET 4+), but perhaps it could.
        /// Presently, comparisons use the default EqualitComparer for TValue,
        /// but they should use a comparer derived from the underlying record list.
        /// </remarks>
        private class ArrayValueEqualityComparer : IEqualityComparer<TValue[]>
        {
            public bool Equals(TValue[] x, TValue[] y)
            {
                bool areEqual = false;

                if (null == x && null == y)
                {
                    areEqual = true;
                }
                else if (null != x && null != y
                    && x.Length == y.Length
                    )
                {
                    int n = x.Length;
                    int i = 0;
                    IEqualityComparer<TValue> baseComparer = EqualityComparer<TValue>.Default;

                    areEqual = true;
                    while (i < n 
                        && areEqual
                        )
                    {
                        areEqual = baseComparer.Equals(x[i], y[i]);
                        i++;
                    }
                }

                return areEqual;
            }

            public int GetHashCode(TValue[] x)
            {
                int compositeHashCode = 0;

                if (null != x)
                {
                    int i;
                    IEqualityComparer<TValue> baseComparer = EqualityComparer<TValue>.Default;
                    for (i = 0; i < x.Length; i++)
                    {
                        int itemHashCode = baseComparer.GetHashCode(x[i]);
                        compositeHashCode = compositeHashCode ^ itemHashCode;
                    }
                }

                return compositeHashCode;
            }
        } // /class


        private class RecordListIndexSearchResultReader
        : IRecordCollectionReader<TValue>
        {
            private readonly IList<int> _baseRecordPositionList;
            private readonly IRecordListCursor<TValue> _baseCursor;
            private int _resultPosition = -1;

            public RecordListIndexSearchResultReader(
                 IList<int> recordPositionList
                ,IRecordListCursor<TValue> baseCursor
                )
            {
                _baseRecordPositionList = recordPositionList;
                _baseCursor = baseCursor;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _baseCursor.Dispose();
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }

            public IRecordAccessor<TValue> Current
            {
                get
                {
                    return _baseCursor.Current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (object)Current;
                }
            }

            public void Close()
            {
                Dispose(true);
            }

            public bool MoveNext()
            {
                bool hasMoved = false;

                if (_resultPosition < _baseRecordPositionList.Count)
                {
                    _resultPosition++;
                }
                if (_resultPosition < _baseRecordPositionList.Count)
                {
                    int recordPosition = _baseRecordPositionList[_resultPosition];
                    hasMoved = _baseCursor.MoveTo(recordPosition);
                }

                return hasMoved;
            }

            public IRecordAccessor<TValue> ReadNextRecord()
            {
                IRecordAccessor<TValue> currentRecord = null;

                if (MoveNext())
                {
                    currentRecord = Current;
                }

                return currentRecord;
            }

            public void Reset()
            {
                _resultPosition = -1;
            }

        } // /class

    } // /class

} // /namespace
