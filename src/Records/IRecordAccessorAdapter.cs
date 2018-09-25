/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
//using System.Collections.Generic;


namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a record accessor that gets its values from some base object
    /// </summary>
    /// <typeparam name="TValue">type for field values of this record</typeparam>
    /// <typeparam name="TBaseRecord">type for the base record</typeparam>
    public interface IRecordAccessorAdapter<TValue,TBaseRecord>
    : IRecordAccessor<TValue>
    {
        /// <summary>
        /// Attach the adapter to a base record
        /// </summary>
        /// <param name="baseRecord">Source record</param>
        void AttachTo(TBaseRecord baseRecord);

    } // /class

} // /namespace
