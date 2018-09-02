/*  Copyright (c) 2016 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

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
    /// <typeparam name="TField"></typeparam>
    /// <remarks>
    /// RecordCollectionReaders created by this object must read records in the order they are stored in the list.
    /// </remarks>
    public interface IRecordList<TValue,TField> 
    : IRecordCollection<TValue,TField>
    {
        /// <summary>
        /// Get a record accessor at a specified position.
        /// Set the values in a record at a specified position.
        /// This is not efficient for loops, use GetRecordListVisitor() in that case.
        /// </summary>
        /// <param name="recordPosition"></param>
        /// <returns></returns>
        IRecordAccessor<TValue> this[int recordPosition] { get; set; }    

        /// <summary>
        /// Insert a copy of record to a position in the list
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
        /// Get an object that will allow efficient random-access to records by their position in the list
        /// </summary>
        /// <returns></returns>
        IRecordListVisitor<TValue> GetRecordListVisitor();

    } // /interface

} // /namespace
