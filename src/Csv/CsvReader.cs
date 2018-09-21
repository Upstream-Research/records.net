/*  Copyright (c) 2017 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Upstream.System.Csv
{
    /// <summary>
    /// Implements a stream-reader type object for parsing a Code-Separated-Value text stream
    /// </summary>
    public class CsvReader
    : IDisposable
     ,IEnumerator<string>
    {
        private readonly CsvEncoding _csvEncoding;
        private TextReader _baseReader;
        private string _currentTextLine;
        private int _currentTextLineIndex;
        private string _currentValueString;
        private StringBuilder _currentValueBuffer;

        /// <summary>
        /// Create a new CSV reader that will wrap a text stream
        /// and will use a specific CSV encoding for parsing the stream.
        /// The CSV record separator will be determined by the base reader's NewLine symbol
        /// </summary>
        /// <param name="baseReader">TextReader object which will be used as the stream source</param>
        /// <param name="csvEncoding">CSV Encoding object used to parse CSV units from the source stream</param>
        public CsvReader(
             TextReader baseReader
            , CsvEncoding csvEncoding
            )
        {
            if (null == baseReader)
            {
                throw new ArgumentNullException("baseReader");
            }
            if (null == csvEncoding)
            {
                throw new ArgumentNullException("csvEncoding");
            }

            _baseReader = baseReader;
            _csvEncoding = csvEncoding;
            _currentTextLine = null;
            _currentTextLineIndex = 0;
            _currentValueString = null;
            _currentValueBuffer = new StringBuilder();
        }

        /// <summary>
        /// Create a new CSV reader that will wrap a text stream and
        /// will use the default CSV encoding.
        /// The CSV record separator will be determined by the base reader's NewLine symbol.
        /// </summary>
        /// <param name="baseReader">TextReader to be wrapped by the CSV reader</param>
        public CsvReader(
            TextReader baseReader
            )
            : this(
                 baseReader
                ,new CsvEncoding()
                )
        {
        }

        /// <summary>
        /// Dispose of resources held by the reader
        /// </summary>
        /// <param name="disposing">True if managed resources should also be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (null != _baseReader)
            {
                if (disposing)
                {
                    _baseReader.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _baseReader = null;
            }
        }

        /// <summary>
        /// Dispose of the reader and the underlying stream, does the same thing as Close()
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close the reader and the underlying stream
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// Get the most recently read unit value from the reader
        /// </summary>
        public string ValueText
        {
            get
            {
                return _currentValueString;
            }
        }

        string IEnumerator<string>.Current
        {
            get
            {
                return ValueText;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return ValueText;
            }
        }

        /// <summary>
        /// Read the beginning of the next record.
        /// Unit values must be read by calling ReadValue() until it returns false.
        /// </summary>
        /// <returns>True if a new record has been read</returns>
        public bool ReadRecord()
        {
            bool hasReadRecord = false;

            if (null == _baseReader)
            {
                throw new InvalidOperationException("reader is closed");
            }
            
            // read any remaining values until we get to the end of the current record
            while (ReadValue());

            // ReadValue() will set _currentTextLine to null after it reads the last value in the record
            if (null == _currentTextLine)
            {
                _currentTextLine = _baseReader.ReadLine();
                _currentTextLineIndex = 0;
                if (null != _currentTextLine)
                {
                    hasReadRecord = true;
                }
            }
            
            return hasReadRecord;
        }

        /// <summary>
        /// Read the next value from the current record
        /// </summary>
        /// <returns>True if a new value (unit) has been read</returns>
        public bool ReadValue()
        {
            bool hasReadValue = false;

            if (null == _baseReader)
            {
                throw new InvalidOperationException("reader is closed");
            }

            if (null != _currentTextLine)
            {
                bool isInsideQuotation = false;
                int readCount;
                int initialTextLineIndex;

                _currentValueBuffer.Clear();
                initialTextLineIndex = _currentTextLineIndex;
                readCount = _csvEncoding.DecodeInto(_currentValueBuffer, ref isInsideQuotation, ref _currentTextLineIndex, _currentTextLine);
                while (isInsideQuotation
                    && null != _currentTextLine
                    )
                {
                    _currentTextLine = _baseReader.ReadLine();
                    _currentTextLineIndex = 0;
                    initialTextLineIndex = _currentTextLineIndex;
                    readCount = 0;
                    if (null != _currentTextLine)
                    {
                        // insert a newline/record-separator between each new line that we decode
                        _currentValueBuffer.Append(_csvEncoding.RecordSeparator);
                        readCount = _csvEncoding.DecodeInto(_currentValueBuffer, ref isInsideQuotation, ref _currentTextLineIndex, _currentTextLine);
                    }
                }

                if (null != _currentTextLine)
                {
                    // if (initialTextLineIndex + readCount < _currentTextLineIndex)
                    // then we read a unit separator, and although there is nothing left to read from the text line,
                    //  there is still an empty value to read back to the client on a subsequent call to this function.
                    if (_currentTextLineIndex >= _currentTextLine.Length
                        && initialTextLineIndex + readCount == _currentTextLineIndex
                        )
                    {
                        // we just read the last value in the record
                        // set _currentTextLine to null to signal that there are no more values to read
                        _currentTextLine = null;
                        _currentTextLineIndex = 0;
                    }
                }
                _currentValueString = _currentValueBuffer.ToString();
                hasReadValue = true;
            }

            return hasReadValue;
        }

        /// <summary>
        /// Iterating the enumerator moves to the next cell
        /// </summary>
        /// <returns></returns>
        bool IEnumerator.MoveNext()
        {
            return ReadValue();
        }

        /// <summary>
        /// Resetting the enumerator tries to move to the next record
        /// </summary>
        void IEnumerator.Reset()
        {
            ReadRecord();
        }

    } // /class

} // /namespace
