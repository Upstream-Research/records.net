/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;


namespace Upstream.System.Json
{
    /// <summary>
    /// Basic JSON Writer interface modeled after NewtonSoft.Json.JsonWriter
    /// </summary>
    public interface IJsonWriter : IDisposable
    {
        /// <summary>
        /// Flush all JSON data from the writer to the backing store
        /// </summary>
        void Flush();

        /// <summary>
        /// Write the opening of a new JSON Object value
        /// </summary>
        void WriteStartObject();

        /// <summary>
        /// Write the closing of the current JSON Object
        /// </summary>
        void WriteEndObject();

        /// <summary>
        /// Write the opening of a new JSON Array value
        /// </summary>
        void WriteStartArray();

        /// <summary>
        /// Write the closing of the current JSON Array
        /// </summary>
        void WriteEndArray();

        /// <summary>
        /// Write the name of a new property in the current JSON Object
        /// </summary>
        /// <param name="propertyName"></param>
        void WritePropertyName(string propertyName);

        /// <summary>
        /// Write a named property with a JSON value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="jsonValue"></param>
        void WriteRawProperty(string propertyName, string jsonValue);

        /// <summary>
        /// Write a property value or an array value (depending on context)
        /// </summary>
        /// <param name="stringValue"></param>
        void WriteValue(String stringValue);

        /// <summary>
        /// Write a property value or an array value (depending on context)
        /// </summary>
        /// <param name="doubleValue"></param>
        void WriteValue(Double? doubleValue);

        /// <summary>
        /// Write a property value or an array value (depending on context)
        /// </summary>
        /// <param name="booleanValue"></param>
        void WriteValue(Boolean? booleanValue);

        /// <summary>
        /// Write a property value or an array value (depending on context)
        /// </summary>
        /// <param name="intValue"></param>
        void WriteValue(Int64? intValue);

    } // /interface

} // /namespace
