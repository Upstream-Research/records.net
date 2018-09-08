/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Globalization;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements an object that can encode and decode values of some generic type as strings
    /// </summary>
    /// <typeparam name="TValue">
    /// Base datatype of the field values that are to be represented by a string
    /// </typeparam>
    public class BasicFieldValueStringRepresentation<TValue>
    : IFieldValueStringRepresentation<TValue>
    {
        private readonly Type _baseDataType;
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Create a new string representation encoding
        /// that will use .NET's Convert and ChangeType functions to manage the string representation
        /// </summary>
        /// <param name="baseDataType">.NET type to use for conversion</param>
        /// <param name="formatProvider">
        /// Format provider, can be a CultureInfo object,
        /// or something more specific like NumberFormatInfo.
        /// </param>
        public BasicFieldValueStringRepresentation(
            Type baseDataType
            ,IFormatProvider formatProvider
            )
        {
            if (null == baseDataType)
            {
                throw new ArgumentNullException("baseDataType");
            }
            if (null == formatProvider)
            {
                throw new ArgumentNullException("cultureInfo");
            }
            _baseDataType = baseDataType;
            _formatProvider = formatProvider;
        }

        /// <summary>
        /// The Type to be associated with this string representation,
        /// should be a type derived from <c>TValue</c>
        /// </summary>
        private Type DataType
        {
            get
            {
                return _baseDataType;
            }
        }

        /// <summary>
        /// IFormatProvider implemenation that will decide the representation of objects
        /// </summary>
        private IFormatProvider StringFormatProvider
        {
            get
            {
                return _formatProvider;
            }
        }

        /// <summary>
        /// Format (i.e. encode, "print") a value as a string.
        /// </summary>
        /// <param name="fieldValue">value to represent in string form</param>
        /// <returns>
        /// string representation of the input value.
        /// </returns>
        public virtual string 
        ToString(TValue fieldValue)
        {
            string stringValue = fieldValue as string;

            if (null == stringValue)
            {
                stringValue = Convert.ToString(fieldValue, StringFormatProvider);
            }

            return stringValue;
        }

        /// <summary>
        /// Try to parse a string into a value.
        /// </summary>
        /// <param name="stringValue">string representation to be parsed</param>
        /// <param name="fieldValue">on exit, value that was parsed from input string</param>
        /// <returns>true if parsing succeeds, false if parsing fails</returns>
        public virtual bool 
        TryParse(string stringValue, out TValue fieldValue)
        {
            bool wasParsed = false;

            fieldValue = default(TValue);
            if (null != stringValue)
            {
                try
                {
                    fieldValue = (TValue)Convert.ChangeType(stringValue, DataType, StringFormatProvider);
                    wasParsed = true;
                }
                catch (InvalidCastException)
                {
                    // Convert.ChangeType can raise InvalidCastException
                    // continue;
                }
                catch (OverflowException)
                {
                    // continue;
                }
                catch (FormatException)
                {
                    // continue;
                }
            }

            return wasParsed;
        }

    } // /interface
} // /namespace
