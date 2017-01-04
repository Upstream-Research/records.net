/*  Copyright (c) 2016 Upstream Research, Inc.  */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a collection type for record accessors.
    /// Doesn't implement IList, but implements a subset of it.
    /// Enumerating this collection does not necessarily return distinct objects
    /// since an IRecordAccessor is a "visitor".
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>
    /// RecordCollectionReaders created by this object must read records in the order they are stored in the list.
    /// </remarks>
    public interface IRecordList<TValue> 
    : IRecordCollection<TValue>
    {
        /// <summary>
        /// Get a record accessor at a specified position.
        /// Set the values in a record at a specified position.
        /// This is not efficient for loops, use OpenRecordListCursor() in that case.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        IRecordAccessor<TValue> this[int recordPosition] { get; set; }    

        /// <summary>
        /// Insert a record to a position in the list
        /// </summary>
        /// <param name="insertPosition"></param>
        /// <param name="record"></param>
        void Insert(int insertPosition, IRecordAccessor<TValue> record);

        /// <summary>
        /// Remove a record from a position in the list
        /// </summary>
        /// <param name="recordPosition"></param>
        void RemoveAt(int recordPosition);

        /// <summary>
        /// Get a cursor that will allow efficient random-access to records by their position in the list
        /// </summary>
        /// <returns></returns>
        IRecordListCursor<TValue> OpenRecordListCursor();

    } // /interface

} // /namespace
