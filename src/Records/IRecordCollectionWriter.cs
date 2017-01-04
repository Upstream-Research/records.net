/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines an object that can write a sequence of records to some back-end record collection
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IRecordCollectionWriter<TValue>
    : IDisposable
     //,ICollector<IRecordAccessor<TValue>>
    {
        /// <summary>
        /// Get the current record that will be written when WriteNextRecord is called.
        /// </summary>
        IRecordAccessor<TValue> Current { get; }

        /// <summary>
        /// Write the current record to the back-end collection.
        /// </summary>
        /// <returns>
        /// Null reference if writing failed.
        /// Returns the same thing as the Current property when writing was successful.
        /// It is possible that writing the record caused some fields to change.
        /// </returns>
        /// <remarks>
        /// This will usually append a new record to the back-end collection.
        /// </remarks>
        IRecordAccessor<TValue> WriteCurrentRecord();

        /// <summary>
        /// Set the fields in the current record to their default values.
        /// </summary>
        /// <returns>The current record associated with this writer object</returns>
        void InitializeCurrentRecord();

        /// <summary>
        /// Close the reader.  Must be called when the caller is finished using the reader.
        /// Does the same thing as the IDisposable.Dispose() method
        /// </summary>
        void Close();
        
    } // /interface

} // /namespace
