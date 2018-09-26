/*  Copyright (c) 2016-2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;
using System.IO;

using NSJ=Newtonsoft.Json;

using UJ=Upstream.System.Json;

namespace NSJson
{
    /// <summary>
    /// Implements a basic JsonWriter interface using NewtonSoft JsonTextWriter
    /// </summary>
    public class JsonTextWriter : UJ.IJsonWriter
    {
        private NSJ.JsonTextWriter _baseJsonWriter;
        // store a stack of property counts for each JSON object we open so that we can handle the WriteRaw case
        private Stack<int> _propertyCountStack;
        private int _propertyCount;

        public JsonTextWriter(TextWriter textWriter)
        {
            _baseJsonWriter = new NSJ.JsonTextWriter(textWriter);
            _propertyCount = 0;
            _propertyCountStack = new Stack<int>();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (null != _baseJsonWriter)
            {
                if (disposing)
                {
                    _baseJsonWriter.Close();
                }

                _baseJsonWriter = null;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        private NSJ.JsonWriter BaseJsonWriter
        {
            get
            {
                return _baseJsonWriter;
            }
        }

        /// <summary>
        /// Set indentation on the JSON text output
        /// </summary>
        public bool IndentationEnabled
        {
            set
            {
                if (value)
                {
                    BaseJsonWriter.Formatting = NSJ.Formatting.Indented;
                }
                else
                {
                    BaseJsonWriter.Formatting = NSJ.Formatting.None;
                }
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Flush()
        {
            BaseJsonWriter.Flush();
        }

        public void WritePropertyName(string propertyName)
        {
            BaseJsonWriter.WritePropertyName(propertyName);
            _propertyCount += 1;
        }

        public void WriteRawProperty(string propertyName, string jsonValue)
        {
            // TODO: this is not the most rebust encoding; there are probably other cases to cover
            // escape backslashes and double quotes
            string encodedPropertyName = propertyName
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");

            // Must write the property name as Raw too since it affects the JsonWriter state
            //  and will cause it to output a "null" right after the raw geometry string.
            //  We must also handle commas between object properties
            if (0 < _propertyCount)
            {
                BaseJsonWriter.WriteRaw(",");
            }
            BaseJsonWriter.WriteRaw(String.Format("\"{0}\":", encodedPropertyName));
            BaseJsonWriter.WriteRaw(jsonValue);

            _propertyCount += 1;
        }

        public void WriteStartArray()
        {
            BaseJsonWriter.WriteStartArray();
        }

        public void WriteEndArray()
        {
            BaseJsonWriter.WriteEndArray();
        }

        public void WriteStartObject()
        {
            BaseJsonWriter.WriteStartObject();
            _propertyCountStack.Push(_propertyCount);
        }

        public void WriteEndObject()
        {
            BaseJsonWriter.WriteEndObject();
            _propertyCount = _propertyCountStack.Pop();
        }

        public void WriteValue(Int64? intValue)
        {
            BaseJsonWriter.WriteValue(intValue);
        }

        public void WriteValue(Boolean? booleanValue)
        {
            BaseJsonWriter.WriteValue(booleanValue);
        }

        public void WriteValue(Double? doubleValue)
        {
            BaseJsonWriter.WriteValue(doubleValue);
        }

        public void WriteValue(String stringValue)
        {
            BaseJsonWriter.WriteValue(stringValue);
        }

    } // /class

} // /namespace
