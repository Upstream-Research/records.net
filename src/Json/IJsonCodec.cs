/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.IO;

namespace Upstream.System.Json
{
    /// <summary>
    /// Abstract factory for creating IJsonReader and IJsonWriter objects (Encoder/Decoder "Codec")
    /// </summary>
    public interface IJsonCodec
    {
        /// <summary>
        /// Factory method to create a new JSON reader on a string of JSON-encoded data
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        IJsonReader CreateJsonReader(string jsonText);

        /// <summary>
        /// Factory method to create a new JSON reader on a stream of text
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        IJsonReader CreateJsonReader(TextReader textReader);

        /// <summary>
        /// Factory method to create a new JSON writer on a stream of text
        /// </summary>
        /// <param name="textWriter"></param>
        /// <returns></returns>
        IJsonWriter CreateJsonWriter(TextWriter textWriter);
    } // /interface

} // /namespace