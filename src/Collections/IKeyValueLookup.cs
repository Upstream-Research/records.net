/*  Copyright (c) 2016 Upstream Research, Inc.  */


using System;
using System.Collections.Generic;

namespace Upstream.System.Collections
{
    /// <summary>
    /// Defines a reduced dictionary interface
    /// </summary>
    public interface IKeyValueLookup<TKey,TValue> : IEnumerable<KeyValuePair<TKey,TValue>>
    {
        /// <summary>
        /// Lookup a value associated with a key.
        /// If the key does not exist, the the behavior is undefined.
        /// Should raise System.Collections.Generic.KeyNotFoundException if the key does not exist
        /// (just like generic IDictionary), but client code should not make this assumption
        /// </summary>
        /// <param name="key"></param>
        TValue this[TKey key] { get; set; }

        /// <summary>
        /// Try to lookup a value, 
        /// if the value doesn't exist then 'false' is returned and a default value is passed back.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="outValue"></param>
        /// <returns>True if a value was found for the specified key, False if the key was not found.</returns>
        bool TryGetValue(TKey key, out TValue outValue);

    } // /class

} // /namespace
