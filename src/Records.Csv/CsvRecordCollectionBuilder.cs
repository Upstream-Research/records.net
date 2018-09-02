/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

using Upstream.System.Csv;

namespace Upstream.System.Records.Csv
{
    /// <summary>
    /// Implements IRecordCollectionBuilder on a CSV writer
    /// </summary>
    public class CsvRecordCollectionBuilder
    : IRecordCollectionBuilder<string>
    {
        private CsvWriter _csvWriter;
        IList<string> _fieldValueList;
        IList<string> _fieldNameList;
        IRecordAccessor<string> _currentRecord;

        /// <summary>
        /// Create a new CSV record collector that will write to a csv writer
        /// </summary>
        /// <param name="csvWriter"></param>
        /// <param name="fieldValueSortComparer"></param>
        /// <param name="fieldValueEqualityComparer"></param>
        /// <param name="fieldNameList"></param>
        public CsvRecordCollectionBuilder(
             CsvWriter csvWriter
            ,IComparer fieldValueSortComparer
            ,IEqualityComparer fieldValueEqualityComparer
            ,IList<string> fieldNameList
            )
        {
            int fieldCount = 0;
            
            fieldCount = fieldNameList.Count;
            BasicRecordSchema<IRecordFieldType<string>> recordSchema = new BasicRecordSchema<IRecordFieldType<string>>();
            foreach (string fieldName in fieldNameList)
            {
                IRecordFieldType<string> fieldType = new BasicRecordFieldType<string>(
                    typeof(String)
                    ,fieldValueSortComparer
                    ,fieldValueEqualityComparer
                    );
                recordSchema.AddField(
                    fieldName
                    ,fieldType
                    );
            }

            _fieldNameList = fieldNameList;
            _fieldValueList = new string[fieldCount];
            _currentRecord = new ListRecordAccessor<string,IRecordFieldType<string>>(
                 recordSchema
                ,_fieldValueList
                );
            _csvWriter = csvWriter;
        }

        /// <summary>
        /// Dispose of the record collector and the underlying csv writer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose of the record collector and the underlying csv writer
        /// </summary>
        /// <param name="notFinalizing"></param>
        protected virtual void Dispose(bool notFinalizing)
        {
            if (null != _csvWriter)
            {
                if (notFinalizing)
                {
                    _csvWriter.Dispose();
                }

                _csvWriter = null;
            }
        }

        /// <summary>
        /// Get the buffer record that will be store the field values to be written
        /// </summary>
        public IRecordAccessor<string> Current
        {
            get
            {
                return _currentRecord;
            }
        }

        /// <summary>
        /// Set the values of all fields in the current record to be null
        /// </summary>
        public void InitializeCurrentItem()
        {
            IList<string> fieldValueList = _fieldValueList;
            int fieldCount = fieldValueList.Count;
            string defaultValue = null;
            int fieldPosition;

            for (fieldPosition = 0; fieldPosition < fieldCount; fieldPosition++)
            {
                fieldValueList[fieldPosition] = defaultValue;
            }
        }

        /// <summary>
        /// Try to write the current record to the backend csv stream
        /// </summary>
        /// <returns></returns>
        public bool AddCurrentItem()
        {
            CsvWriter csvWriter = _csvWriter;
            IList<string> fieldValueList = _fieldValueList;
            int fieldCount = fieldValueList.Count;
            int fieldPosition;

            csvWriter.WriteStartRecord();

            for (fieldPosition = 0; fieldPosition < fieldCount; fieldPosition++)
            {
                string fieldValue = fieldValueList[fieldPosition];
                csvWriter.WriteValue(fieldValue);
            }

            csvWriter.WriteEndRecord();

            return true;
        }

        /// <summary>
        /// Write a new record to the CSV stream
        /// </summary>
        /// <param name="recordItem"></param>
        /// <returns></returns>
        public bool Add(IRecordAccessor<string> recordItem)
        {
            if (null == recordItem)
            {
                return false;
            }
            
            IList<string> fieldNameList = _fieldNameList;
            IList<string> fieldValueList = _fieldValueList;
            int fieldPosition;

            for (fieldPosition = 0
                ;fieldPosition < fieldNameList.Count
                    && fieldPosition < fieldValueList.Count
                ;fieldPosition++
            )
            {
                string fieldName = fieldNameList[fieldPosition];
                string fieldValue;
                recordItem.TryGetValue(fieldName, out fieldValue);
                fieldValueList[fieldPosition] = fieldValue;
            }

            return AddCurrentItem();
        }



    } // /class

} // /namespace
