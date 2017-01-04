/*  Copyright (c) 2016 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides an efficient, enumerator-like "cursor" with random-access to a record list.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// The implementation should reuse a the current RecordAccessor object as the cursor
    /// moves over the list to avoid unnecessary creation of wrapper objects (visitor pattern).
    /// </remarks>
    public interface IRecordListCursor<TValue>
    : IDisposable
     , IEnumerator<IRecordAccessor<TValue>>
     , IRecordCollectionReader<TValue>
    {
        /// <summary>
        /// Set the fields in the current record to their default values.
        /// </summary>
        /// <returns>The current record associated with this writer object</returns>
        void InitializeCurrentRecord();

        /// <summary>
        /// Does the same thing as Seek(int), but called more like an IEnumerator method.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        bool MoveTo(int recordPosition);
        
        /// <summary>
        /// Try to move the cursor to a new position in the record list.
        /// If the cursor cannot be moved, null is returned and the Current record
        /// does not change.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns>null if the record position could not be accessed.</returns>
        IRecordAccessor<TValue> Seek(int recordPosition);

    } // /interface

} // /namespace

