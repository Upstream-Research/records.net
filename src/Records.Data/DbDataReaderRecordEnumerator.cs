/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Data;


namespace Upstream.System.Records.Data
{
    /// <summary>
    /// Implements IRecordEnumerator with a .NET IDataReader
    /// </summary>
    public class DbDataReaderRecordEnumerator
    : IRecordEnumerator<object>
    {
        IDataReader _baseReader;
        DbDataRecordAccessor _recordAccessor = new DbDataRecordAccessor();

        /// <summary>
        /// Create a record enumerator that is not attached to a data reader
        /// </summary>
        public DbDataReaderRecordEnumerator()
        {
        }

        /// <summary>
        /// Create a data reader record enumerator that is attached to a data reader instance
        /// </summary>
        /// <param name="baseReader"></param>
        public DbDataReaderRecordEnumerator(
            IDataReader baseReader
            )
        {
            AttachTo(baseReader);
        }

        /// <summary>
        /// Dispose of this enumerator and the underlying reader
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose of this object and the underlying reader
        /// </summary>
        /// <param name="managedDisposal"></param>
        protected virtual void Dispose(bool managedDisposal)
        {
            if (null != _baseReader)
            {
                if (managedDisposal)
                {
                    _baseReader.Dispose();
                }

                _baseReader = null;
            }
        }

        /// <summary>
        /// Attach this record enumerator to a data reader object
        /// </summary>
        /// <param name="baseReader"></param>
        public void AttachTo(IDataReader baseReader)
        {
            IDataRecord baseRecord = baseReader;
            _recordAccessor.AttachTo(baseRecord);
            _baseReader = baseReader;
        }

        /// <summary>
        /// Get the base data reader.
        /// Returns a null reference if no base reader is attached.
        /// </summary>
        public IDataReader DataReader
        {
            get
            {
                return _baseReader;
            }
        }

        /// <summary>
        /// Get an accessor for the current data record
        /// </summary>
        public IRecordAccessor<object> Current
        {
            get
            {
                return _recordAccessor;
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
        /// Try to get the next record
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            bool hasMoved = false;
            IDataReader reader = DataReader;

            if (null != reader)
            {
                hasMoved = reader.Read();
            }
            
            return hasMoved;
        }

        /// <summary>
        /// Implements IEnumerator.Reset(),
        /// but raises an exception since a data reader cannot be reset.
        /// </summary>
        public void Reset()
        {
            throw new InvalidOperationException("cannot reset data reader");
        }
    } // /class

} // /namespace
