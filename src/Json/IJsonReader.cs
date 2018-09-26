/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;

namespace Upstream.System.Json
{
    /// <summary>
    /// Defines a basic reader interface for JSON-encoded data.
    /// JSON data is read in as "elements" which may be simple values, lists (arrays) of unnamed values,
    /// or a list (object) of named values (i.e. properties).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation deliberately avoids using a "Token" enum,
    /// even though most JSON reader implementations seem to have one.
    ///  One goal with this design was to make it easy to implement the signature of this interface 
    ///  without having to use custom types.  This allows the pattern of use to be reused
    ///  even if the types cannot be reused]
    /// </para>
    /// </remarks>
    public interface IJsonReader : IDisposable
    {
        /// <summary>
        /// If the last read operation read a new property element, then this gets the property name.
        /// If the last read operation read a new array value, then this should return a null reference.
        /// In any other case, then the returned value could be anything (or an exception might be raised).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Determine if the start of a property list object was read in the last read operation
        /// </summary>
        bool IsAtObjectStart { get; }

        /// <summary>
        /// Determine if the end of a property list object was read in the last read operation
        /// </summary>
        bool IsAtObjectEnd { get; }

        /// <summary>
        /// Determine if the start of a value array was read in the last read operation
        /// </summary>
        bool IsAtArrayStart { get; }

        /// <summary>
        /// Determine if the end of a value array was read in the last read operation
        /// </summary>
        bool IsAtArrayEnd { get; }

        /// <summary>
        /// Determine if a simple data value was read in the last read operation.
        /// The actual value can be retrieved from the Value property.
        /// If the simple value is a named "property" on a JSON property list object element, 
        /// then the value's name can be retrieved from the Name property.
        /// </summary>
        bool IsAtSimpleValue { get; }

        /// <summary>
        /// If the last read operation read a simple property element or a simple array value, then this is the value.
        /// </summary>
        /// <remark>
        /// If the last read operation read a simple null value, then this should return a null reference (not DBNull)
        /// (however, it is possible that an implementation may require DBNull be returned regardless, 
        /// so if the client wants to be very robust, it should check for DBNull)
        /// </remark>
        T GetValue<T>();
        
        /// <summary>
        /// Try to read a sibling or child element from the underlying stream.
        /// If the end of the current object or array element is reached, then this function returns false.
        /// Use ReadUp() to go back up to the parent element.
        /// An element can be: a simple value (named or unnamed), 
        /// the start of a child JSON array (list) element, 
        /// the start of a child JSON object (property list) element.
        /// </summary>
        /// <returns>
        /// True if a JSON element was read. 
        /// False if the end of the current complex element was read (or end of the stream).
        /// </returns>
        bool ReadDown();

        /// <summary>
        /// Read up to the parent element.  If there is more sibling or child content, then it will be skipped.
        /// </summary>
        /// <returns>
        /// True if the reader moved to the parent element successfully
        /// </returns>
        bool ReadUp();
       
    } // /interface

} // /namespace
