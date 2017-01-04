/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a disposable iterator over a record collection
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IRecordCollectionReader<TValue>
    : IDisposable
     , IEnumerator<IRecordAccessor<TValue>>
    {
        /// <summary>
        /// Read the next record from the collection.
        /// Has the same effect as IEnumerator.MoveNext.
        /// </summary>
        /// <returns>
        /// Null reference if there are no more records to read.
        /// Returns the same thing as the IEnumerator.Current property.
        /// </returns>
        IRecordAccessor<TValue> ReadNextRecord();

        /// <summary>
        /// Close the reader.  Must be called when the caller is finished using the reader.
        /// Does the same thing as the IDisposable.Dispose() method
        /// </summary>
        void Close();

    } // /interface

} // /namespace
