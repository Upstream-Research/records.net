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
    /// Implements Upstream.System.Json.IJsonReader using Newtonsoft JsonTextReader.
    /// </summary>
    public class JsonTextReader : UJ.IJsonReader
    {
        private NSJ.JsonReader _baseJsonReader;
        private string _propertyName = null;
        private object _value = null;
        private bool _canReadPastEndToken = false;

        public JsonTextReader(TextReader textReader)
        {
            NSJ.JsonTextReader baseJsonTextReader = new NSJ.JsonTextReader(textReader);
            _baseJsonReader = baseJsonTextReader;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (null != _baseJsonReader)
            {
                if (disposing)
                {
                    _baseJsonReader.Close();
                }

                _baseJsonReader = null;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        private NSJ.JsonReader BaseJsonReader
        {
            get
            {
                return _baseJsonReader;
            }
        }

        public string Name
        {
            get
            {
                return _propertyName;
            }
        }


        public bool IsAtObjectStart
        {
            get
            {
                return (NSJ.JsonToken.StartObject == BaseJsonReader.TokenType);
            }
        }

        public bool IsAtObjectEnd
        {
            get
            {
                return (NSJ.JsonToken.EndObject == BaseJsonReader.TokenType);
            }
        }
        
        public bool IsAtArrayStart
        {
            get
            {
                return (NSJ.JsonToken.StartArray == BaseJsonReader.TokenType);
            }
        }

        public bool IsAtArrayEnd
        {
            get
            {
                return (NSJ.JsonToken.EndArray == BaseJsonReader.TokenType);
            }
        }

        public bool IsAtSimpleValue
        {
            get
            {
                NSJ.JsonToken jsonTokenType = BaseJsonReader.TokenType;

                return (NSJ.JsonToken.Integer == jsonTokenType
                    || NSJ.JsonToken.Float == jsonTokenType
                    || NSJ.JsonToken.String == jsonTokenType
                    || NSJ.JsonToken.Boolean == jsonTokenType
                    || NSJ.JsonToken.Null == jsonTokenType
                    || NSJ.JsonToken.Undefined == jsonTokenType
                    || NSJ.JsonToken.Date == jsonTokenType
                    || NSJ.JsonToken.Bytes == jsonTokenType
                    );
            }
        }
        
        public T GetValue<T>()
        {
            object nodeValueObject = _value;
            T nodeValue = (T)nodeValueObject;
            
            return nodeValue;
        }

        public bool ReadDown()
        {
            bool hasRead = false;
            bool endOfElement = false;
            NSJ.JsonToken jsonTokenType = BaseJsonReader.TokenType;
            string propertyName = null;

            if (NSJ.JsonToken.EndObject == jsonTokenType
                || NSJ.JsonToken.EndArray == jsonTokenType
                )
            {
                if (false == _canReadPastEndToken)
                {
                    endOfElement = true;
                }
            }

            while (false == hasRead
                && false == endOfElement 
                && BaseJsonReader.Read()
                )
            {
                jsonTokenType = BaseJsonReader.TokenType;
                if (NSJ.JsonToken.EndArray == jsonTokenType)
                {
                    endOfElement = true;
                }
                else if (NSJ.JsonToken.EndObject == jsonTokenType)
                {
                    endOfElement = true;
                }
                else if (NSJ.JsonToken.PropertyName == jsonTokenType)
                {
                    propertyName = BaseJsonReader.Value as String;
                }
                else if (NSJ.JsonToken.StartObject == jsonTokenType)
                {
                    hasRead = true;
                }
                else if (NSJ.JsonToken.StartArray == jsonTokenType)
                {
                    hasRead = true;
                }
                else if (NSJ.JsonToken.Integer == jsonTokenType
                    || NSJ.JsonToken.Float == jsonTokenType
                    || NSJ.JsonToken.String == jsonTokenType
                    || NSJ.JsonToken.Boolean == jsonTokenType
                    || NSJ.JsonToken.Null == jsonTokenType
                    || NSJ.JsonToken.Undefined == jsonTokenType
                    || NSJ.JsonToken.Date == jsonTokenType
                    || NSJ.JsonToken.Bytes == jsonTokenType
                    )
                {
                    _value = BaseJsonReader.Value;
                    hasRead = true;
                }

                // _propertyName will get set to null if we don't read a property name
                _propertyName = propertyName;
            }

            return hasRead;
        }

        public bool ReadUp()
        {
            int depth = 1;
            
            do
            {
                NSJ.JsonToken jsonTokenType = BaseJsonReader.TokenType;
                if (NSJ.JsonToken.StartArray == jsonTokenType
                    || NSJ.JsonToken.StartObject == jsonTokenType
                    || NSJ.JsonToken.StartConstructor == jsonTokenType
                    )
                {
                    depth += 1;
                }
                else if (NSJ.JsonToken.EndArray == jsonTokenType
                    || NSJ.JsonToken.EndObject == jsonTokenType
                    || NSJ.JsonToken.EndConstructor == jsonTokenType
                    )
                {
                    depth -= 1;
                    if (0 == depth)
                    {
                        // this will allow the next ReadDown() operation to read past this end token
                        _canReadPastEndToken = true;
                    }
                }
            } while (0 < depth
                && BaseJsonReader.Read()
                );
            
            return _canReadPastEndToken;
        }

        public void Close()
        {
            Dispose();
        }

    } // /class

} // /namespace
