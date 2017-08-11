/*  Copyright (c) 2017 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Upstream.System.Csv
{
    /// <summary>
    /// Implements a TextWriter wrapper for writing CSV "records" consisting of "unit" values.
    /// Records are delimited by the newline supported by the underlying text writer.
    /// </summary>
    public class CsvWriter
    : IDisposable
    {
        private readonly CsvEncoding _csvEncoding;
        private readonly StringBuilder _unitBuffer;

        private TextWriter _baseWriter;
        private int _unitCount;

        /// <summary>
        /// Create a CSV Writer that will wrap a TextWriter
        /// and that will use a particular CSV encoding (format)
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="csvEncoding"></param>
        public CsvWriter(
             TextWriter textWriter
            ,CsvEncoding csvEncoding
            )
        {
            if (null == textWriter)
            {
                throw new ArgumentNullException("textWriter");
            }
            if (null == csvEncoding)
            {
                throw new ArgumentNullException("csvEncoding");
            }

            _baseWriter = textWriter;
            _csvEncoding = csvEncoding;
            _unitBuffer = new StringBuilder();
            _unitCount = 0;
        }

        /// <summary>
        /// Create a CSV Writer that will wrap a TextWriter
        /// and will use a default CSV encoding
        /// </summary>
        /// <param name="textWriter"></param>
        public CsvWriter(
            TextWriter textWriter
            )
        : this(
            textWriter
            ,new CsvEncoding()
            )
        {
        }

        /// <summary>
        /// Dispose of retained resources
        /// </summary>
        /// <param name="disposing">true if called from a Dispose() or Close() method,
        /// false if called from a finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (null != _baseWriter)
            {
                if (disposing)
                {
                    _baseWriter.Dispose();
                }
                
                _baseWriter = null;
            }
        }

        /// <summary>
        /// Close the writer and the underlying stream
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close the writer and the underlying stream
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// Signal the beginning of a new record.
        /// Values in the record must be written with the WriteValue() method.
        /// </summary>
        public void WriteStartRecord()
        {
            if (null == _baseWriter)
            {
                throw new InvalidOperationException("writer is closed");
            }

            _unitCount = 0;
        }

        /// <summary>
        /// Signal the end of the current record.
        /// </summary>
        public void WriteEndRecord()
        {
            if (null == _baseWriter)
            {
                throw new InvalidOperationException("writer is closed");
            }

            _baseWriter.WriteLine();
            _unitCount = 0;
        }

        /// <summary>
        /// Write a unit value for the current record
        /// </summary>
        public void WriteValue(string valueString)
        {
            if (null == _baseWriter)
            {
                throw new InvalidOperationException("writer is closed");
            }

            _unitBuffer.Clear();
            _csvEncoding.EncodeValueInto(_unitBuffer, valueString);
            string encodedValueString = _unitBuffer.ToString();
            if (0 < _unitCount)
            {
                _baseWriter.Write(_csvEncoding.UnitSeparator);
            }
            _baseWriter.Write(encodedValueString);
            _unitCount++;
        }

    } // /class

} // /namespace
