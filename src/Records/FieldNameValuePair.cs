/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements a readonly, covariant generic interface of a KeyValuePair with a string key
    /// </summary>
    /// <typeparam name="TValue">type of value associated with the named record field</typeparam>
    public struct FieldNameValuePair<TValue>
    : IFieldNameValuePair<TValue>
    {
        private readonly string _name;
        private readonly TValue _value;

        /// <summary>
        /// Initialize a FieldNameValuePair with a field name and its value
        /// </summary>
        /// <param name="fieldName">name of field</param>
        /// <param name="fieldValue">value assigned to field</param>
        public FieldNameValuePair(
            string fieldName
            ,TValue fieldValue
            )
        {
            _name = fieldName;
            _value = fieldValue;
        }

        /// <summary>
        /// Name of the field
        /// </summary>
        public string Name 
        { 
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Value assigned to the field
        /// </summary>
        public TValue Value 
        { 
            get
            {
                return _value;
            } 
        }

    } // /class

} // /namespace
