/*  Copyright (c) 2017 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines an enumerator-like interface for stepping over a collection of records.
    /// Implements an enumerator interface, but the "current" object is assumed to be
    /// an unchanging reference to a "visitor" object which changes its state as the enumerator moves.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IRecordEnumerator<TValue>
    : IDisposable
     ,IEnumerator<IRecordAccessor<TValue>>
    {
        
    } // /interface

} // /namespace
