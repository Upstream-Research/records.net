﻿/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements a record list on a list of record field value arrays
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ArrayRecordList<TValue> 
    : IRecordCollection<TValue>
     ,IList<IRecordAccessor<TValue>>
    {
        private IList<string> _fieldNameList = new List<string>();
        private IList<TValue[]> _fieldValueListList = new List<TValue[]>();

        /// <summary>
        /// Create a new record collection using the specified field names
        /// </summary>
        /// <param name="fieldNameEnumeration"></param>
        public ArrayRecordList(
            IEnumerable<string> fieldNameEnumeration
            )
        {
            if (null != fieldNameEnumeration)
            {
                foreach (string fieldName in fieldNameEnumeration)
                {
                    _fieldNameList.Add(fieldName);
                }
            }
        }

        /// <summary>
        /// List of ordered field names for records in this record list
        /// </summary>
        private IList<string> FieldNameList
        {
            get
            {
                return _fieldNameList;
            }
        }

        /// <summary>
        /// Base record list, which is a list of field value arrays
        /// </summary>
        private IList<TValue[]> BaseRecordList
        {
            get
            {
                return _fieldValueListList;
            }
        }

        /// <summary>
        /// Get the number of records in this collection
        /// </summary>
        public Int32 Count
        {
            get
            {
                return BaseRecordList.Count;
            }
        }

        /// <summary>
        /// Get the number of fields in each record
        /// </summary>
        public Int32 FieldCount
        {
            get
            {
                return FieldNameList.Count;
            }
        }

        /// <summary>
        /// Get an enumeration of field names for records in this collection
        /// </summary>
        public IEnumerable<String> FieldNames
        {
            get
            {
                return (IEnumerable<string>)FieldNameList;
            }
        }

        /// <summary>
        /// This collection is not readonly
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get/Set a record accessor from the collection.
        /// This is not an efficient way to access the record collection in a loop,
        /// Use a RecordCollection Reader or Writer if possible.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        public IRecordAccessor<TValue> this[int recordPosition]
        {
            get
            {
                if (recordPosition < 0)
                {
                    throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position cannot be negative");
                }
                if (recordPosition > BaseRecordList.Count)
                {
                    throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position is too large");
                }
                TValue[] fieldValueArray = BaseRecordList[recordPosition];
                IRecordAccessor<TValue> recordAccessor = new ListRecordAccessor<TValue>(FieldNameList, fieldValueArray);

                return recordAccessor;
            }
            set
            {
                IRecordAccessor<TValue> recordAccessor = value;

                if (recordPosition < 0)
                {
                    throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position cannot be negative");
                }
                if (recordPosition > BaseRecordList.Count)
                {
                    throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position is too large");
                }
                TValue[] fieldValueArray = BaseRecordList[recordPosition];
                CopyRecordFieldValuesInto(fieldValueArray, recordAccessor);
            }
        }

        /// <summary>
        /// Insert a new record into the list
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <param name="inputRecord"></param>
        public void Insert(int recordPosition, IRecordAccessor<TValue> inputRecord)
        {
            if (recordPosition < 0)
            {
                throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position cannot be negative");
            }
            if (recordPosition > BaseRecordList.Count)
            {
                throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position is too large");
            }
            if (null == inputRecord)
            {
                throw new ArgumentNullException("inputRecord");
            }

            TValue[] fieldValueArray = CreateRecordFieldValueArray(inputRecord);
            BaseRecordList.Insert(recordPosition, fieldValueArray);
        }

        /// <summary>
        /// Add a copy of the data in a record to this collection
        /// </summary>
        /// <param name="inputRecord"></param>
        public void Add(IRecordAccessor<TValue> inputRecord)
        {
            if (null == inputRecord)
            {
                throw new ArgumentNullException("inputRecord");
            }

            TValue[] fieldValueArray = CreateRecordFieldValueArray(inputRecord);
            BaseRecordList.Add(fieldValueArray);
        }

        /// <summary>
        /// Factory method to create a new record by copying fields from another record
        /// </summary>
        /// <param name="inputRecord"></param>
        /// <returns></returns>
        private TValue[] 
        CreateRecordFieldValueArray(IRecordAccessor<TValue> inputRecord)
        {
            int fieldCount = FieldNameList.Count;
            TValue[] fieldValueArray = new TValue[fieldCount];

            CopyRecordFieldValuesInto(fieldValueArray, inputRecord);

            return fieldValueArray;
        }

        /// <summary>
        /// Centralized function to copy field values from an external record into
        /// an internal field value array record
        /// </summary>
        /// <param name="fieldValueArray"></param>
        /// <param name="inputRecord"></param>
        private void
        CopyRecordFieldValuesInto(
             TValue[] fieldValueArray
            ,IRecordAccessor<TValue> inputRecord
            )
        {
            int fieldCount = FieldNameList.Count;
            int fieldOrdinal;

            if (null != inputRecord
                && null != fieldValueArray
                )
            {
                for (fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
                {
                    string fieldName = FieldNameList[fieldOrdinal];
                    TValue fieldValue;
                    if (inputRecord.TryGetValue(fieldName, out fieldValue))
                    {
                        fieldValueArray[fieldOrdinal] = fieldValue;
                    }
                }
            }
        }

        /// <summary>
        /// Centralized method to initialize a record to its default values
        /// </summary>
        /// <param name="fieldValueList"></param>
        private void 
        InitializeRecord(IList<TValue> fieldValueList)
        {
            if (null == fieldValueList)
            {
                return;
            }

            int fieldCount = fieldValueList.Count;
            int fieldOrdinal;
            for (fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
            {
                fieldValueList[fieldOrdinal] = default(TValue);
            }
        }

        /// <summary>
        /// Delete all records from this collection
        /// </summary>
        public void Clear()
        {
            BaseRecordList.Clear();
        }

        /// <summary>
        /// Scan the collection to find a record that exactly matches the data for the fields in the input record.
        /// If the input record has fields that don't apply to this collection, then nothing can be found.
        /// If the input record has less fields than the fields in this collection,
        /// then a match will be done only on the fields in the input record.
        /// </summary>
        /// <param name="inputRecord"></param>
        /// <returns></returns>
        public Boolean Contains(IRecordAccessor<TValue> inputRecord)
        {
            return (0 <= IndexOf(inputRecord));
        }

        /// <summary>
        /// This is a .NET method made for performance efficiency,
        /// but in our case it would be very inefficient to fill an array of record object wrappers,
        /// so you should not use this function unless you have a really good reason.
        /// </summary>
        /// <param name="targetArray"></param>
        /// <param name="targetStartPosition"></param>
        public void 
        CopyTo(
             IRecordAccessor<TValue>[] targetArray
            ,int targetStartPosition
            )
        {
            if (null == targetArray)
            {
                throw new ArgumentNullException("targetArray");
            }
            if (0 > targetStartPosition)
            {
                throw new ArgumentOutOfRangeException("targetStartPosition");
            }

            int sourcePosition;
            int targetPosition;
            for (sourcePosition = 0
                 ,targetPosition = targetStartPosition
                ; sourcePosition < BaseRecordList.Count
                && targetPosition < targetArray.Length
                ; sourcePosition++
                 ,targetPosition++
                )
            {
                IList<TValue> fieldValueList = BaseRecordList[sourcePosition];
                targetArray[targetPosition] = new ListRecordAccessor<TValue>(FieldNameList, fieldValueList);
            }
        }

        /// <summary>
        /// Remove a record by its position in the list
        /// </summary>
        /// <param name="recordPosition"></param>
        public void RemoveAt(int recordPosition)
        {
            if (0 > recordPosition)
            {
                throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position cannot be negative");
            }
            if (BaseRecordList.Count <= recordPosition)
            {
                throw new ArgumentOutOfRangeException("recordPosition", recordPosition, "position is too large");
            }

            BaseRecordList.RemoveAt(recordPosition);
        }

        /// <summary>
        /// Remove all records that match the input record.
        /// If the input record has fields that don't apply to this collection, then nothing can be found.
        /// If the input record has less fields than the fields in this collection,
        /// then a match will be done only on the fields in the input record.
        /// </summary>
        /// <param name="inputRecord"></param>
        /// <returns>true if anything was removed</returns>
        public Boolean Remove(IRecordAccessor<TValue> inputRecord)
        {
            int recordIndex;
            int removedRecordCount = 0;

            recordIndex = IndexOf(inputRecord);
            while (0 <= recordIndex)
            {
                BaseRecordList.RemoveAt(recordIndex);
                removedRecordCount++;
                recordIndex = IndexOf(inputRecord, recordIndex);
            }

            return (0 < removedRecordCount);
        }
        

        /// <summary>
        /// Create a record cursor to find records in this collection
        /// </summary>
        /// <returns></returns>
        public IRecordListVisitor<TValue> 
        GetRecordListVisitor()
        {
            // This will return an updatable cursor
            return new ArrayRecordListVisitor(this);
        }

        /// <summary>
        /// Create a record reader to scan all records in this collection
        /// </summary>
        /// <returns></returns>
        public IRecordEnumerator<TValue> 
        GetRecordEnumerator()
        {
            // This will return an updatable cursor
            return GetRecordListVisitor();
        }

        /// <summary>
        /// Create a record writer that will append records to this collection
        /// </summary>
        /// <returns></returns>
        public IRecordCollectionBuilder<TValue> OpenRecordCollectionBuilder()
        {
            return new ArrayRecordListBuilder(this);
        }

        /// <summary>
        /// Enumerate the records in this collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IRecordAccessor<TValue>> GetEnumerator()
        {
            // This will return an updatable cursor.
            // We can return this object from GetEnumerator() since it doesn't really need to be disposed of.
            return new ArrayRecordListVisitor(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Scan the collection to find a record that exactly matches the data for the fields in the input record.
        /// If the input record has fields that don't apply to this collection, then nothing can be found.
        /// If the input record has less fields than the fields in this collection,
        /// then a match will be done only on the fields in the input record.
        /// </summary>
        /// <param name="inputRecord"></param>
        /// <returns></returns>
        public int IndexOf(
            IRecordAccessor<TValue> inputRecord
            )
        {
            int startSearchIndex = 0;
            return IndexOf(inputRecord, startSearchIndex);
        }

        /// <summary>
        /// Scan the collection to find a record that exactly matches the data for the fields in the input record.
        /// If the input record has fields that don't apply to this collection, then nothing can be found.
        /// If the input record has less fields than the fields in this collection,
        /// then a match will be done only on the fields in the input record.
        /// </summary>
        /// <param name="inputRecord"></param>
        /// <param name="startSearchIndex"></param>
        /// <returns>negative value if no record was found</returns>
        private int IndexOf(
             IRecordAccessor<TValue> inputRecord
            ,int startSearchIndex
            )
        {
            int NotFoundIndex = -1;
            int foundIndex = NotFoundIndex;

            if (null == inputRecord)
            {
                return NotFoundIndex;
            }

            IEnumerator<string> inputFieldNameEnumerator = inputRecord.GetFieldNameEnumerator();
            ListRecordAccessor<TValue> record = new ListRecordAccessor<TValue>(FieldNameList);
            int recordIndex = startSearchIndex;
            while (recordIndex < BaseRecordList.Count
                && NotFoundIndex == foundIndex
                )
            {
                record.FieldValueList = BaseRecordList[recordIndex];

                // assume fields are equal until we find one that isn't
                bool fieldsAreEqual = true;
                while (fieldsAreEqual
                    && inputFieldNameEnumerator.MoveNext()
                    )
                {
                    string fieldName = inputFieldNameEnumerator.Current;
                    TValue inputFieldValue = inputRecord[fieldName];
                    fieldsAreEqual = (0 == record.CompareFieldTo(fieldName, inputFieldValue));
                }

                if (fieldsAreEqual)
                {
                    foundIndex = recordIndex;
                }
                else
                {
                    inputFieldNameEnumerator.Reset();
                    recordIndex++;
                }
            }

            return foundIndex;
        }

        /// <summary>
        /// Implements a record reader over this record collection
        /// </summary>
        private class ArrayRecordListVisitor
        : IRecordListVisitor<TValue>
        {
            private readonly ArrayRecordList<TValue> BaseList;
            private readonly ListRecordAccessor<TValue> BaseRecordAccessor;
            private int CurrentIndex = -1;

            internal ArrayRecordListVisitor(ArrayRecordList<TValue> baseList)
            {
                if (null == baseList)
                {
                    throw new ArgumentNullException("baseList");
                }
                BaseList = baseList;
                BaseRecordAccessor = new ListRecordAccessor<TValue>(baseList.FieldNameList);
            }

            public void Dispose()
            {
            }

            /// <summary>
            /// Get the current record from the reader
            /// </summary>
            public IRecordAccessor<TValue> Current
            {
                get
                {
                    return BaseRecordAccessor;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }


            public bool MoveNext()
            {
                bool hasMoved = false;
                int recordPosition = CurrentIndex;

                // Notice that this will allow the CurrentIndex to fall out of sync
                //  with the BaseRecordAccessor.
                // When this happens, it is supposed to mean that the enumerator is at the end of the enumeration.
                // Also note that CurrentIndex, and thus recordPosition can be set to -1
                if (recordPosition < BaseList.BaseRecordList.Count)
                {
                    recordPosition++;
                    hasMoved = MoveTo(recordPosition);
                }

                return hasMoved;
            }

            /// <summary>
            /// Centralized function to move the cursor
            /// </summary>
            /// <param name="recordPosition"></param>
            /// <returns></returns>
            public bool  MoveTo(int recordPosition)
            {
                bool hasMoved = false;

                if (0 <= recordPosition
                    && BaseList.BaseRecordList.Count > recordPosition
                    )
                {
                    BaseRecordAccessor.FieldValueList = BaseList.BaseRecordList[recordPosition];
                    CurrentIndex = recordPosition;
                    hasMoved = true;
                }

                return hasMoved;
            }

            /// <summary>
            /// Moves to a point just before the first record
            /// </summary>
            public void Reset()
            {
                BaseRecordAccessor.FieldValueList = null;
                CurrentIndex = -1;
            }

            public void InitializeCurrentItem()
            {
                if (null != BaseRecordAccessor.FieldValueList)
                {
                    BaseList.InitializeRecord(BaseRecordAccessor.FieldValueList);
                }
            }
        } // /class


        /// <summary>
        /// Implements a record collection writer on an array record collection
        /// </summary>
        private class ArrayRecordListBuilder
        : IRecordCollectionBuilder<TValue>
        {
            private readonly ArrayRecordList<TValue> BaseList;
            private readonly ListRecordAccessor<TValue> BaseRecordAccessor;
            private IList<TValue> BaseFieldValueList;

            internal ArrayRecordListBuilder(ArrayRecordList<TValue> baseList)
            {
                if (null == baseList)
                {
                    throw new ArgumentNullException("baseList");
                }
                BaseList = baseList;
                BaseFieldValueList = new TValue[BaseList.FieldCount];
                BaseRecordAccessor = new ListRecordAccessor<TValue>(baseList.FieldNameList, BaseFieldValueList);
            }

            public void Dispose()
            {
            }
            
            public IRecordAccessor<TValue> Current
            {
                get
                {
                    return BaseRecordAccessor;
                }
            }

            public void InitializeCurrentItem()
            {
                BaseList.InitializeRecord(BaseFieldValueList);
            }

            public bool 
            AddCurrentItem()
            {
                // copy the current field values into a new field value array and add it to the collection,
                //  leave the current field value list alone so that the client can continue to modify it
                int fieldCount = BaseFieldValueList.Count;
                TValue[] fieldValueArray = new TValue[fieldCount];
                int fieldOrdinal;
                for (fieldOrdinal = 0; fieldOrdinal < fieldCount; fieldOrdinal++)
                {
                    fieldValueArray[fieldOrdinal] = BaseFieldValueList[fieldOrdinal];
                }
                BaseList.BaseRecordList.Add(fieldValueArray);

                return true;
            }


            public bool 
            Add(IRecordAccessor<TValue> record)
            {
                bool recordWasAdded = false;

                if (null != record)
                {
                    BaseList.Add(record);
                    recordWasAdded = true;
                }

                return recordWasAdded;
            }

        } // /class

    } // /class
} // /namespace
