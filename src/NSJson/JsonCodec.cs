/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.IO;

//using NSJ=Newtonsoft.Json;

using UJ=Upstream.System.Json;

namespace NSJson
{
    /// <summary>
    /// Implements Upstream IJsonCodec with reader/writer objects based on Newtonsoft JSON
    /// </summary>
    public class JsonCodec : UJ.IJsonCodec
    {
        private bool _indentationEnabled = false;

        /// <summary>
        /// Set indentation on the JSON text output
        /// </summary>
        public bool IndentationEnabled
        {
            get
            {
                return _indentationEnabled;
            }
            set
            {
                _indentationEnabled = value;
            }
        }

        /// <summary>
        /// Factory method to create a JsonReader
        /// </summary>
        /// <param name="textReader"></param>
        /// <returns></returns>
        public UJ.IJsonReader CreateJsonReader(TextReader textReader)
        {
            return new JsonTextReader(textReader);
        }

        /// <summary>
        /// Factory method to create a JsonReader
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public UJ.IJsonReader CreateJsonReader(string jsonText)
        {
            TextReader textReader = new StringReader(jsonText);
            return CreateJsonReader(textReader);
        }

        /// <summary>
        /// Factory method to create a JsonWriter
        /// </summary>
        /// <param name="textWriter"></param>
        /// <returns></returns>
        public UJ.IJsonWriter CreateJsonWriter(TextWriter textWriter)
        {
            JsonTextWriter jsonWriter = new JsonTextWriter(textWriter);
            jsonWriter.IndentationEnabled = IndentationEnabled;

            return jsonWriter;
        }
    } // /class

} // /namespace
