/*  Copyright (c) 2016-2017 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides the obvious implementation for IRecordFieldProperties
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class BasicRecordFieldType<TValue>
    : IRecordFieldType<TValue>
    {
        private Type _dataType;
        private IComparer _sortComparer;
        private IEqualityComparer _equalityComparer;

        /// <summary>
        /// Create a new field meta-properties object
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="sortComparer"></param>
        /// <param name="equalityComparer"></param>
        public BasicRecordFieldType(
             Type dataType
            ,IComparer sortComparer
            ,IEqualityComparer equalityComparer
            )
        {
            if (null == dataType)
            {
                throw new ArgumentNullException("dataType");
            }
            if (null == sortComparer)
            {
                throw new ArgumentNullException("sortComparer");
            }
            if (null == equalityComparer)
            {
                throw new ArgumentNullException("equalityComparer");
            }

            _dataType = dataType;
            _sortComparer = sortComparer;
            _equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Create a field descriptor that will use the default characteristics of a data type
        /// </summary>
        /// <param name="dataType"></param>
        public BasicRecordFieldType(
            Type dataType
            )
        : this(
             dataType
            ,Comparer<TValue>.Default
            ,EqualityComparer<TValue>.Default
            )
        {
        }

        /// <summary>
        /// Create a field descriptor that will use the default characteristics of the base field value type
        /// </summary>
        public BasicRecordFieldType()
        : this(typeof(TValue))
        {
        }

        /// <summary>
        /// Get the .NET type that is compatible with this field
        /// </summary>
        public Type DataType
        {
            get
            {
                return _dataType;
            }
        }

        /// <summary>
        /// Get an object that can compare two values for this field to determine their sort order
        /// </summary>
        private IComparer ValueSortComparer
        {
            get
            {
                return _sortComparer;
            }
        }

        /// <summary>
        /// Get an object that can compare two field values for equivalence
        /// and can be used for generating dictionary keys for values of this field.
        /// </summary>
        private IEqualityComparer ValueEqualityComparer
        {
            get
            {
                return _equalityComparer;
            }
        }

        /// <summary>
        /// Determine if a field value is valid for this field type
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool IsValid(TValue fieldValue)
        {
            bool isValid = false;

            if (null == fieldValue)
            {
                isValid = true;
            }
            else
            {
                Type fieldValueType = fieldValue.GetType();
                // [20170810 [db] IsAssignableFrom() would be a better method to use,
                //  but it requires the System.Reflection extensions in .NET Core]
                if (DataType.Equals(fieldValueType))
                {
                    isValid = true;
                }
            }

            return isValid;
        }

        /// <summary>
        /// compare two field values for their sort order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(TValue x, TValue y)
        {
            return ValueSortComparer.Compare(x, y);
        }

        /// <summary>
        /// Compare two field values for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TValue x, TValue y)
        {
            return ValueEqualityComparer.Equals(x, y);
        }

        /// <summary>
        /// Get an equality hash code for a field value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TValue obj)
        {
            return ValueEqualityComparer.GetHashCode(obj);
        }
    } // /class

} // /namespace
