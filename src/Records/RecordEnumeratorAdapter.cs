/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IRecordEnumerator over a base enumerator using a record adapter
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TBaseRecord"></typeparam>
    public class RecordEnumeratorAdapter<TValue, TBaseRecord>
    : IRecordEnumerator<TValue>
    {
        private readonly IRecordAccessorAdapter<TValue,TBaseRecord> _recordAdapter;
        private IEnumerator<TBaseRecord> _baseEnumerator;

        /// <summary>
        /// Create a new record enumerator that will visit records in a base enumerator
        /// and expose them with a record adapter
        /// </summary>
        /// <param name="recordAdapter"></param>
        /// <param name="baseEnumerator"></param>
        public RecordEnumeratorAdapter(
            IRecordAccessorAdapter<TValue,TBaseRecord> recordAdapter
            ,IEnumerator<TBaseRecord> baseEnumerator
            )
        {
            if (null == recordAdapter)
            {
                throw new ArgumentNullException("recordAdapter");
            }

            _recordAdapter = recordAdapter;
            _baseEnumerator = baseEnumerator;
        }

        /// <summary>
        /// Dispose of this enumerator and the base enumerator
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose of enumerator and base enumerator
        /// </summary>
        /// <param name="notFinalizing"></param>
        protected virtual void Dispose(bool notFinalizing)
        {
            if (null != _baseEnumerator)
            {
                if (notFinalizing)
                {
                    _baseEnumerator.Dispose();
                }

                _baseEnumerator = null;
            }
        }

        /// <summary>
        /// Get the current record
        /// </summary>
        public IRecordAccessor<TValue> Current
        {
            get
            {
                return _recordAdapter;
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
        /// Try to move to the next record
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            bool hasMoved = false;
            IEnumerator<TBaseRecord> baseEnumerator = _baseEnumerator;
            IRecordAccessorAdapter<TValue,TBaseRecord> recordAdapter = _recordAdapter;

            if (null != baseEnumerator
                && baseEnumerator.MoveNext()
                )
            {
                recordAdapter.AttachTo(baseEnumerator.Current);
                hasMoved = true;
            }

            return hasMoved;
        }

        /// <summary>
        /// Try to reset the enumerator to a point before the first record
        /// </summary>
        public void Reset()
        {
            IEnumerator<TBaseRecord> baseEnumerator = _baseEnumerator;
            IRecordAccessorAdapter<TValue,TBaseRecord> recordAdapter = _recordAdapter;

            if (null != baseEnumerator)
            {
                baseEnumerator.Reset();
            }
        }

    } // /class

} // /namespace
