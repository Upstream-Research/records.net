/*  Copyright (c) 2016 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Collections
{
    /// <summary>
    /// Implements a KeyValueLookup with a generic Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryKeyValueLookup<TKey, TValue> : IKeyValueLookup<TKey, TValue>
    {
        private IDictionary<TKey,TValue> _baseDictionary;

        /// <summary>
        /// Create a new KeyValueLookup from an existing dictionary
        /// </summary>
        /// <param name="baseDictionary"></param>
        public DictionaryKeyValueLookup(IDictionary<TKey,TValue> baseDictionary)
        {
            _baseDictionary = baseDictionary;
        }

        /// <summary>
        /// Get the Dictionary that implements this KeyValueLookup
        /// </summary>
        public IDictionary<TKey,TValue> BaseDictionary
        {
            get
            {
                return _baseDictionary;
            }
            set
            {
                _baseDictionary = value;
            }
        }

        /// <summary>
        /// Get/Set an item in the lookup table
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                TValue returnValue;
                BaseDictionary.TryGetValue(key, out returnValue);

                return returnValue;
            }

            set
            {
                BaseDictionary[key] = value;
            }
        }

        /// <summary>
        /// Try to find the value for a key in the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue outValue)
        {
            return BaseDictionary.TryGetValue(key, out outValue);
        }

        /// <summary>
        /// Get an enumerator of all keys and values
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
        {
            return BaseDictionary.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator of all keys and values as a non-generic IEnumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    } // /class

} // /namespace
