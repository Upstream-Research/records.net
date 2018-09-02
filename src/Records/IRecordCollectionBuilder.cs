/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
//using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a collector-type interface that provides a working-record
    /// that can be written-back to a record collection
    /// </summary>
    public interface IRecordCollectionBuilder<TValue>
    : IRecordCollector<TValue>
    {
        /// <summary>
        /// Get the current working item which can be modified before the record is added
        /// </summary>
        IRecordAccessor<TValue> Current { get; }

        /// <summary>
        /// Initialize the state of the current object
        /// (set fields to default values)
        /// </summary>
        void InitializeCurrentItem();

        /// <summary>
        /// Try to add the current item to the underlying collection
        /// </summary>
        /// <returns>
        /// true if the item was successfully added, otherwise false
        /// </returns>
        bool AddCurrentItem();

    } // /interface
} // /class
