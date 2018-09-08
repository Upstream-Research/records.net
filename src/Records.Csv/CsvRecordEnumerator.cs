/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

using Upstream.System.Csv;


namespace Upstream.System.Records.Csv
{
    /// <summary>
    /// Implements IRecordEnumerator on a CSV reader that reads string values only
    /// </summary>
    public class CsvRecordEnumerator
    : IRecordEnumerator<string>
    {
        private CsvReader _csvReader;
        private IList<string> _fieldValueList;
        //private ListRecordAccessor<string,IRecordFieldType<string>> _currentRecord;
        private IRecordAccessor<string> _currentRecord;

        /// <summary>
        /// Create a new record enumerator that will read records from the provided CSV stream
        /// </summary>
        /// <param name="csvReader">Base CSV reader</param>
        /// <param name="fieldNameEnumeration">Names of fields that will be exposed by this record reader</param>
        /// <param name="fieldValueSortComparer">The sort comparer to be associated with field types in the record enumerator's record schema</param>
        /// <param name="fieldValueEqualityComparer">The equality comparer to be associated with field types in the record enumerator's record schema</param>
        public CsvRecordEnumerator(
             CsvReader csvReader
            ,IEnumerable<string> fieldNameEnumeration
            ,IComparer fieldValueSortComparer
            ,IEqualityComparer fieldValueEqualityComparer
            )
        {
            int fieldCount = 0;
            
            if (null == fieldNameEnumeration)
            {
                throw new ArgumentNullException("fieldNameEnumeration");
            }

            BasicRecordSchema<IRecordFieldType<string>> recordSchema = new BasicRecordSchema<IRecordFieldType<string>>();
            foreach (string fieldName in fieldNameEnumeration)
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
                fieldCount += 1;
            }

            _fieldValueList = new string[fieldCount];
            _currentRecord = new ListRecordAccessor<string,IRecordFieldType<string>>(
                 recordSchema
                ,_fieldValueList
                );
            _csvReader = csvReader;
        }

        /// <summary>
        /// Create a new record enumerator that will read records from the provided CSV stream
        /// </summary>
        /// <param name="csvReader">Base CSV reader</param>
        /// <param name="fieldNameEnumeration">Names of fields that will be exposed by this record reader</param>
        public CsvRecordEnumerator(
             CsvReader csvReader
            ,IEnumerable<string> fieldNameEnumeration
            )
            : this(
                csvReader
                ,fieldNameEnumeration
                ,StringComparer.Ordinal
                ,StringComparer.Ordinal
                )
        {
        }

        /// <summary>
        /// Dispose of the enumerator and the underlying CSV reader
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the enumerator and the underlying CSV reader
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (null != _csvReader)
            {
                if (disposing)
                {
                    _csvReader.Dispose();
                }

                _csvReader = null;
            }
        }

        /// <summary>
        /// Get the current record
        /// </summary>
        public IRecordAccessor<string> Current
        {
            get
            {
                return _currentRecord;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _currentRecord;
            }
        }

        /// <summary>
        /// Raises InvalidOperationException
        /// </summary>
        public void Reset()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Try to read the next record
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            CsvReader csvReader = _csvReader;
            IList<string> fieldValueList = _fieldValueList;
            int fieldPosition;
            int fieldCount = fieldValueList.Count;
            bool hasRead = false;
            string defaultFieldValue = null;

            if (csvReader.ReadRecord())
            {
                fieldPosition = 0;
                while (fieldPosition < fieldCount
                    && csvReader.ReadValue()
                    )
                {
                    string fieldValue = csvReader.ValueText;
                    fieldValueList[fieldPosition] = fieldValue;
                    fieldPosition += 1;
                }
                // if there are missing columns in the CSV, then fill them out with a default value
                while (fieldPosition < fieldCount)
                {
                    fieldValueList[fieldPosition] = defaultFieldValue;
                    fieldPosition += 1;
                }

                hasRead = true;
            }

            return hasRead;
        }


    } // /class

} // /namespace
