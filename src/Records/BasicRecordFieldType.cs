/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides the obvious implementation for BasicRecordFieldType
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class BasicRecordFieldType<TValue>
    : IRecordFieldType<TValue>
    {
        private Type _systemType;
        private IComparer<TValue> _sortComparer;
        private IEqualityComparer<TValue> _equalityComparer;

        /// <summary>
        /// Create a new field type object
        /// </summary>
        /// <param name="systemType">
        /// Type which should be assigned to non-null values of associated with fields of this type.
        /// </param>
        /// <param name="sortComparer">
        /// Comparer to use when sorting records using fields associated with this type.
        /// </param>
        /// <param name="equalityComparer">
        /// Comparer to use when searching and indexing records using fields associated with this type
        /// </param>
        public BasicRecordFieldType(
             Type systemType
            ,IComparer<TValue> sortComparer
            ,IEqualityComparer<TValue> equalityComparer
            )
        {
            if (null == systemType)
            {
                //throw new ArgumentNullException("dataType");
            }
            if (null == sortComparer)
            {
                throw new ArgumentNullException("sortComparer");
            }
            if (null == equalityComparer)
            {
                throw new ArgumentNullException("equalityComparer");
            }

            _systemType = systemType;
            _sortComparer = sortComparer;
            _equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Create a field descriptor that will use the default characteristics of a data type
        /// </summary>
        /// <param name="systemType">
        /// System Type associated with values in fields of this type.
        /// </param>
        public BasicRecordFieldType(
            Type systemType
            )
        : this(
             systemType
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
        public Type SystemType
        {
            get
            {
                return _systemType;
            }
        }

        /// <summary>
        /// Get an object that can compare two values for this field to determine their sort order
        /// </summary>
        private IComparer<TValue> ValueSortComparer
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
        private IEqualityComparer<TValue> ValueEqualityComparer
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
                if (SystemType.Equals(fieldValueType))
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
