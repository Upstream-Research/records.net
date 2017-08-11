/*  Copyright (c) 2016 Upstream Research, Inc.  */

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
    public interface IRecordListVisitor<TValue>
    : IRecordEnumerator<TValue>
    {
        /// <summary>
        /// Set the fields in the current record to their default values.
        /// </summary>
        /// <returns>The current record associated with this visitor object</returns>
        void InitializeCurrentItem();

        /// <summary>
        /// Seek to a specific record by its offset in the record list.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        bool MoveTo(int recordPosition);
        
    } // /interface

} // /namespace

