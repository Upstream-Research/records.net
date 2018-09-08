/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines an object that can encode and decode values of some generic type as strings
    /// </summary>
    /// <typeparam name="TValue">
    /// Base datatype of the field values that are to be represented by a string
    /// </typeparam>
    public interface IFieldValueStringRepresentation<TValue>
    {
        /// <summary>
        /// Format (i.e. encode, "print") a value as a string.
        /// </summary>
        /// <param name="fieldValue">the value to represent as a string</param>
        /// <returns>string representation of the value</returns>
        string ToString(TValue fieldValue);

        /// <summary>
        /// Try to parse a string into a value.
        /// </summary>
        /// <param name="formattedValue">the value represented in string form</param>
        /// <param name="parsedValue">on exit, the value decoded from a string</param>
        /// <returns></returns>
        bool TryParse(string formattedValue, out TValue parsedValue);

    } // /interface
} // /namespace
