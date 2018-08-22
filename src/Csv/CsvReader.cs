/*  Copyright (c) 2017 Upstream Research, Inc.  */

using System;
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
        /// <param name="baseReader"></param>
        /// <param name="csvEncoding"></param>
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
        /// <param name="baseReader"></param>
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
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Read the beginning of the next record.
        /// Unit values must be read by calling ReadValue() until it returns false.
        /// </summary>
        /// <returns></returns>
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
        /// <returns></returns>
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
                int readCount = 0;

                _currentValueBuffer.Clear();
                readCount = _csvEncoding.DecodeInto(_currentValueBuffer, ref isInsideQuotation, _currentTextLine, _currentTextLineIndex);
                while (isInsideQuotation
                    && null != _currentTextLine
                    )
                {
                    _currentTextLine = _baseReader.ReadLine();
                    _currentTextLineIndex = 0;
                    readCount = 0;
                    if (null != _currentTextLine)
                    {
                        // insert a newline/record-separator between each new line that we decode
                        _currentValueBuffer.Append(_csvEncoding.RecordSeparator);
                        readCount = _csvEncoding.DecodeInto(_currentValueBuffer, ref isInsideQuotation, _currentTextLine, _currentTextLineIndex);
                    }
                }
                _currentTextLineIndex += readCount;

                if (null != _currentTextLine)
                {
                    if (_currentTextLineIndex >= _currentTextLine.Length)
                    {
                        // we just read the last value in the record
                        // set _currentTextLine to null to signal that there are no more values to read
                        _currentTextLine = null;
                        _currentTextLineIndex = 0;
                    }
                    else
                    {
                        // ASSERT _csvEncoding.EqualsUnitSeparator(_currentTextLine, _currentTextLineIndex)
                        if (_csvEncoding.EqualsUnitSeparator(_currentTextLine, _currentTextLineIndex))
                        {
                            _currentTextLineIndex += _csvEncoding.UnitSeparator.Length;
                        }
                    }
                }
                _currentValueString = _currentValueBuffer.ToString();
                hasReadValue = true;
            }

            return hasReadValue;
        }

    } // /class

} // /namespace
