/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IEnumerable for parallel lists of field names and field values.
    /// This class can be used as a "helper" object when invoking constructors of other classes.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class FieldNameValuePairEnumeration<TValue>
    : IEnumerable<IFieldNameValuePair<TValue>>
    {
        private readonly IEnumerable<string> _fieldNameEnumeration;
        private readonly IEnumerable<TValue> _fieldValueEnumeration;

        /// <summary>
        /// Create a new enumeration of FieldNameValuePair objects
        /// using parallel enumerations of the field names and values.
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <param name="fieldValues"></param>
        public FieldNameValuePairEnumeration(
            IEnumerable<string> fieldNames
            ,IEnumerable<TValue> fieldValues
            )
        {
            if (null == fieldNames)
            {
                throw new ArgumentNullException("fieldNames");
            }
            if (null == fieldValues)
            {
                throw new ArgumentNullException("fieldValues");
            }

            _fieldNameEnumeration = fieldNames;
            _fieldValueEnumeration = fieldValues;
        }

        /// <summary>
        /// Get an enumerator of the field names and values
        /// "zipped" into FieldNameValuePairs
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<TValue>> GetEnumerator()
        {
            IEnumerator<string> fieldNameEnumerator = _fieldNameEnumeration.GetEnumerator();
            IEnumerator<TValue> fieldValueEnumerator = _fieldValueEnumeration.GetEnumerator();

            while (fieldNameEnumerator.MoveNext()
                && fieldValueEnumerator.MoveNext()
                )
            {
                yield return new FieldNameValuePair<TValue>(
                    fieldNameEnumerator.Current
                    ,fieldValueEnumerator.Current
                    );
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // //class
} // /namespace
