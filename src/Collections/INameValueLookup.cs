/*  Copyright (c) 2016 Upstream Research, Inc.  */


using System;
using System.Collections.Generic;

namespace Upstream.System.Collections
{
    /// <summary>
    /// Defines a reduced dictionary interface for string-key lookups
    /// </summary>
    public interface INameValueLookup<TValue> : IKeyValueLookup<string,TValue>
    {
    } // /class

} // /namespace
