/*  Copyright (c) 2017 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements a "collector" object which can add records to a collection
    /// </summary>
    public interface IRecordCollector<TValue>
    : IDisposable
    {
        /// <summary>
        /// Add a new record to the underlying record store.
        /// Copies the compatible fields of the input record into a new record and saves it.
        /// </summary>
        /// <param name="recordItem"></param>
        /// <returns>
        /// True if the item was successfully added
        /// </returns>
        bool Add(IRecordViewer<TValue> recordItem);

    } // /interface

}  // /namespace
