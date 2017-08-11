/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides methods for parsing a field schema spec mini language
    /// </summary>
    /// <remarks>
    /// The field schema spec mini language is a comma-separated list
    /// of field names with optional type information.
    /// Type information is appended to the field name after a colon character.
    /// For example: "id:varchar,name:varchar,age:int32".
    /// Recognized datatype names can be customized.
    /// </remarks>
    public class FieldSchemaSpec<TValue>
    {
        readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
        private readonly IDictionary<string,Type> _dataTypeDictionary;
        private readonly Type _defaultDataType = typeof(Object);

        /// <summary>
        /// Create a new field schema spec parser that will recognize a very basic set of datatypes 
        /// (varchar, int32, int64, float32, float64, decimal, boolean, varbinary, guid, datetime, timespan)
        /// </summary>
        public FieldSchemaSpec()
        {
            _dataTypeDictionary = CreateDataTypeDictionary();

            AddDefaultDataTypesTo(_dataTypeDictionary);
        }

        /// <summary>
        /// Create a new field schema spec parser that will recognize
        /// a specific set of field data types
        /// </summary>
        /// <param name="dataTypeNameEnumeration"></param>
        public FieldSchemaSpec(
            IEnumerable<KeyValuePair<string,Type>> dataTypeNameEnumeration
            )
        {
            _dataTypeDictionary = CreateDataTypeDictionary();

            IDictionary<string,Type> dataTypeDictionary = _dataTypeDictionary;
            if (null != dataTypeNameEnumeration)
            {
                foreach (KeyValuePair<string,Type> dataTypeNamePair in dataTypeNameEnumeration)
                {
                    string dataTypeName = dataTypeNamePair.Key;
                    Type dataType = dataTypeNamePair.Value;

                    dataTypeDictionary[dataTypeName] = dataType;
                }
            }
        }

        /// <summary>
        /// Centralized factory method
        /// </summary>
        /// <returns></returns>
        private static IDictionary<string,Type> 
        CreateDataTypeDictionary()
        {
            IEqualityComparer<string> dataTypeNameComparer = StringComparer.OrdinalIgnoreCase;
            return new Dictionary<string,Type>(dataTypeNameComparer);
        }

        /// <summary>
        /// Add some common datatype names to the name-type registry
        /// </summary>
        /// <param name="dataTypeDictionary"></param>
        /// <remarks>
        /// The recognized datatypes here are meant to be quite minimal.
        /// It is tempting to support a lot of alternate names 
        /// (e.g. "string", "text", in addition to "varchar"),
        /// but if you really want these names it is probably best to make a custom type mapping
        /// in order to avoid potential version incompatiblities and other sorts of surprises.
        /// </remarks>
        private void AddDefaultDataTypesTo(
            IDictionary<string,Type> dataTypeDictionary
            )
        {
            Type dataType;

            // [20170322 [db] Considered calling this "string" or "text",
            //  but settled on "varchar" to maintain parity with "varbinary" (below).]
            dataType = typeof(String);
            dataTypeDictionary["varchar"] = dataType;

            dataType = typeof(Int32);
            dataTypeDictionary["int32"] = dataType;

            dataType = typeof(Int64);
            dataTypeDictionary["int64"] = dataType;

            dataType = typeof(Double);
            dataTypeDictionary["float64"] = dataType;

            dataType = typeof(Single);
            dataTypeDictionary["float32"] = dataType;

            dataType = typeof(Decimal);
            dataTypeDictionary["decimal"] = dataType;

            dataType = typeof(Boolean);
            dataTypeDictionary["boolean"] = dataType;

            // [20170322 [db] Chose "varbinary" since it seemed like the most conventional
            //  term (from sql server anyway) for this.  
            //  Postgresql's "bytea" doesn't seem very intuitive,
            //  "byte_array" is too clumsy, "bytes" is unconventional, 
            //  and "blob" is probably misleading]
            dataType = typeof(byte[]);
            dataTypeDictionary["varbinary"] = dataType;

            dataType = typeof(Guid);
            dataTypeDictionary["guid"] = dataType;

            dataType = typeof(DateTime);
            dataTypeDictionary["datetime"] = dataType;

            dataType = typeof(TimeSpan);
            dataTypeDictionary["timespan"] = dataType;
        }

        /// <summary>
        /// Centralized function to lookup a datatype from its name
        /// </summary>
        /// <param name="fieldTypeName"></param>
        /// <param name="defaultDataType"></param>
        /// <returns></returns>
        private Type
        GetDataTypeForFieldTypeName(
            string fieldTypeName
            ,Type defaultDataType
            )
        {
            IDictionary<string,Type> dataTypeDictionary = _dataTypeDictionary;
            Type dataType;
            
            if (!dataTypeDictionary.TryGetValue(fieldTypeName, out dataType))
            {
                dataType = defaultDataType;
            }

            return dataType;
        }

        /// <summary>
        /// Parse a field schema spec into an enumeration of record field type objects
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string,FieldSchemaSpecFieldRecord<TValue>>> 
        ParseEnumerable(
            string fieldSchemaSpecString
        )
        {
            return new FieldSchemaSpecEnumeration(this, fieldSchemaSpecString);
        }

        /// <summary>
        /// Parse a single field description
        /// </summary>
        /// <param name="fieldSchemaSpecString"></param>
        /// <param name="startPosition"></param>
        /// <param name="internalBuffer">a buffer to use during parsing,
        /// if a buffer is not provided, then a new buffer will be allocated</param>
        /// <returns></returns>
        public KeyValuePair<string,FieldSchemaSpecFieldRecord<TValue>>
        ParseField(
            string fieldSchemaSpecString
            ,int startPosition = 0
            ,StringBuilder internalBuffer = null
            )
        {
            StringBuilder buffer = internalBuffer;
            string fieldName;
            FieldSchemaSpecFieldRecord<TValue> fieldType;
            
            if (null == buffer)
            {
                buffer = new StringBuilder();
            }
            else
            {
                internalBuffer.Clear();
            }

            ParseField(
                fieldSchemaSpecString
                ,startPosition
                ,out fieldName
                ,out fieldType
                ,buffer
                );
            
            return new KeyValuePair<string,FieldSchemaSpecFieldRecord<TValue>>(fieldName,fieldType);
        }

        /// <summary>
        /// Parse a field out of a field spec string
        /// </summary>
        /// <param name="fieldSchemaSpecString"></param>
        /// <param name="startPosition"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldType"></param>
        /// <param name="buffer">A string builder that will be used for internal buffering</param>
        /// <returns>number of characters read, including any terminating field delimiter</returns>
        private int 
        ParseField(
             string fieldSchemaSpecString
            ,int startPosition
            ,out string fieldName
            ,out FieldSchemaSpecFieldRecord<TValue> fieldType
            ,StringBuilder buffer
            )
        {
            string specString = fieldSchemaSpecString;
            Type defaultDataType = _defaultDataType;
            string fieldDelimiter = ",";
            string typeDelimiter =  ":";
            string optionStartDelimiter = "(";
            string optionEndDelimiter = ")";
            string unspecifiedFieldTypeName = "";
            string unspecifiedFieldOptionSpec = "";
            int charPosition;
            int charCount = 0;
            int symbolCharCount;
            string fieldTypeName;
            string fieldOptionSpec;
            bool endOfField;
            Func<string,int,string,bool> SubstringEquals =
            (string s, int i, string s2) =>
            {
                return (0 == String.Compare(s, i, s2, 0, s2.Length, StringComparison.Ordinal));
            };
            
            if (null == buffer)
            {
                throw new ArgumentNullException("buffer");
            }
            buffer.Clear();

            // default output parameter values
            fieldName = null;
            fieldType = null;

            if (null != specString)
            {
                fieldName = null;
                fieldTypeName = null;
                fieldOptionSpec = null;
                endOfField = false;
                charPosition = startPosition;
                while (charPosition < specString.Length
                    && false == endOfField
                    )
                {
                    symbolCharCount = 1;
                    if (null == fieldName)
                    {
                        if (SubstringEquals(specString, charPosition, fieldDelimiter))
                        {
                            fieldName = buffer.ToString();
                            buffer.Clear();
                            symbolCharCount = fieldDelimiter.Length;
                            endOfField = true;
                        }
                        else if (SubstringEquals(specString, charPosition, typeDelimiter))
                        {
                            fieldName = buffer.ToString();
                            buffer.Clear();
                            symbolCharCount = typeDelimiter.Length;
                        }
                        else
                        {
                            symbolCharCount = 1;
                            buffer.Append(specString, charPosition, symbolCharCount);
                        }
                    }
                    else if (null == fieldTypeName)
                    {
                        if (SubstringEquals(specString, charPosition, fieldDelimiter))
                        {
                            fieldTypeName = buffer.ToString();
                            buffer.Clear();
                            symbolCharCount = fieldDelimiter.Length;
                            endOfField = true;
                        }
                        else if (SubstringEquals(specString, charPosition, optionStartDelimiter))
                        {
                            fieldTypeName = buffer.ToString();
                            buffer.Clear();
                            symbolCharCount = optionStartDelimiter.Length;
                        }
                        else
                        {
                            symbolCharCount = 1;
                            buffer.Append(specString, charPosition, symbolCharCount);
                        }
                    }
                    else if (null == fieldOptionSpec)
                    {
                        if (SubstringEquals(specString, charPosition, optionEndDelimiter))
                        {
                            fieldOptionSpec = buffer.ToString();
                            buffer.Clear();
                            symbolCharCount = optionEndDelimiter.Length;
                        }
                        else
                        {
                            symbolCharCount = 1;
                            buffer.Append(specString, charPosition, symbolCharCount);
                        }
                    }
                    else
                    {
                        if (SubstringEquals(specString, charPosition, fieldDelimiter))
                        {
                            endOfField = true;
                            symbolCharCount = fieldDelimiter.Length;
                        }
                        else
                        {
                            // ignore this char
                            symbolCharCount = 1;
                        }
                    }

                    charCount += symbolCharCount;
                    charPosition = startPosition + charCount;

                    if (charPosition >= specString.Length)
                    {
                        if (null == fieldName)
                        {
                            fieldName = buffer.ToString();
                            buffer.Clear();
                        }
                        else if (null == fieldTypeName)
                        {
                            fieldTypeName = buffer.ToString();
                            buffer.Clear();
                        }
                        endOfField = true;
                    }

                    if (endOfField
                        && null != fieldName
                        )
                    {
                        if (null == fieldTypeName)
                        {
                            fieldTypeName = unspecifiedFieldTypeName;
                        }
                        if (null == fieldOptionSpec)
                        {
                            fieldOptionSpec = unspecifiedFieldOptionSpec;
                        }

                        fieldTypeName = fieldTypeName.Trim();
                        Type dataType = GetDataTypeForFieldTypeName(fieldTypeName, defaultDataType);

                        fieldName = fieldName.Trim();
                        fieldType = new FieldSchemaSpecFieldRecord<TValue>(
                             fieldName
                            ,fieldTypeName
                            ,dataType
                            );
                    }
                }
            }

            return charCount;
        }


        /// <summary>
        /// Implements an IEnumerable interface that parses a field schema spec string
        /// </summary>
        private class FieldSchemaSpecEnumeration
        : IEnumerable<KeyValuePair<string,FieldSchemaSpecFieldRecord<TValue>>>
        {
            private readonly FieldSchemaSpec<TValue> _parser;
            private string _fieldSchemaSpecString;

            internal FieldSchemaSpecEnumeration(
                 FieldSchemaSpec<TValue> parser
                ,string fieldSchemaSpecString
                )
            {
                _parser = parser;
                _fieldSchemaSpecString = fieldSchemaSpecString;
            }

            public IEnumerator<KeyValuePair<string,FieldSchemaSpecFieldRecord<TValue>>>
            GetEnumerator()
            {
                string specString = _fieldSchemaSpecString;
                int startPosition = 0;
                int charPosition;
                int charCount;
                StringBuilder buffer = new StringBuilder();
                
                if (null != specString)
                {
                    charPosition = startPosition;
                    while (charPosition < specString.Length)
                    {
                        string fieldName;
                        FieldSchemaSpecFieldRecord<TValue> fieldType;

                        charCount = _parser.ParseField(
                             specString
                            ,charPosition
                            ,out fieldName
                            ,out fieldType
                            ,buffer
                            );
                        if (0 >= charCount)
                        {
                            // couldn't parse anything, break out of the loop
                            charPosition = specString.Length;
                        }
                        else
                        {
                            charPosition += charCount;
                        }
                        if (null != fieldName
                            && null != fieldType
                            )
                        {
                            yield return new KeyValuePair<string,FieldSchemaSpecFieldRecord<TValue>>(fieldName, fieldType);
                        }

                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        } // /class
    } // /class

} // /namespace
