/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;
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
    public class FieldSchemaSpecEncoding<TValue>
    {
        /// <summary>
        /// Dictionary to lookup a Type for a type name.
        /// There may be multiple names for a Type.
        /// </summary>
        private readonly IDictionary<string,Type> _dataTypeDictionary;

        /// <summary>
        /// Dictionary to lookup a default name for a Type
        /// </summary>
        private readonly IDictionary<Type,string> _dataTypeNameDictionary;

        private readonly Type _defaultDataType = typeof(Object);

        /// <summary>
        /// Create a new field schema spec encoding that will recognize a very basic set of datatypes 
        /// (varchar, int32, int64, float32, float64, decimal, boolean, varbinary, guid, datetime, timespan)
        /// </summary>
        public FieldSchemaSpecEncoding()
        {
            _dataTypeDictionary = CreateDataTypeDictionary();
            _dataTypeNameDictionary = CreateDataTypeNameDictionary();

            AddDefaultDataTypesTo(_dataTypeDictionary, _dataTypeNameDictionary);
        }

        /// <summary>
        /// Create a new field schema spec encoding that will recognize
        /// a specific set of field data types
        /// </summary>
        /// <param name="dataTypeNameEnumeration">an enumerable collection of key-value pairs
        /// which associate a field name (in the field spec) to a .NET datatype
        /// </param>
        public FieldSchemaSpecEncoding(
            IEnumerable<KeyValuePair<string,Type>> dataTypeNameEnumeration
            )
        {
            _dataTypeDictionary = CreateDataTypeDictionary();
            _dataTypeNameDictionary = CreateDataTypeNameDictionary();

            IDictionary<string,Type> dataTypeDictionary = _dataTypeDictionary;
            IDictionary<Type,string> dataTypeNameDictionary = _dataTypeNameDictionary;
            if (null != dataTypeNameEnumeration)
            {
                foreach (KeyValuePair<string,Type> dataTypeNamePair in dataTypeNameEnumeration)
                {
                    string dataTypeName = dataTypeNamePair.Key;
                    Type dataType = dataTypeNamePair.Value;

                    dataTypeDictionary[dataTypeName] = dataType;

                    // set the default type name the first time we see the type,
                    //  after this, all other names for this type will be non-default names
                    if (!dataTypeNameDictionary.ContainsKey(dataType))
                    {
                        dataTypeNameDictionary[dataType] = dataTypeName;
                    }
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
        /// Centralized factory method
        /// </summary>
        private static IDictionary<Type,string>
        CreateDataTypeNameDictionary()
        {
            return new Dictionary<Type,string>();
        }

        /// <summary>
        /// Add some common datatype names to the name-type registry
        /// </summary>
        /// <param name="dataTypeDictionary"></param>
        /// <param name="dataTypeNameDictionary"></param>
        /// <remarks>
        /// The recognized datatypes here are meant to be quite minimal.
        /// It is tempting to support a lot of alternate names 
        /// (e.g. "string", "text", in addition to "varchar"),
        /// but if you really want these names it is probably best to make a custom type mapping
        /// in order to avoid potential version incompatiblities and other sorts of surprises.
        /// </remarks>
        private void AddDefaultDataTypesTo(
            IDictionary<string,Type> dataTypeDictionary
            ,IDictionary<Type,string> dataTypeNameDictionary
            )
        {
            IList<KeyValuePair<string,Type>> dataTypeList = new KeyValuePair<string,Type>[] {
                // [20170322 [db] Considered calling this "string" or "text",
                //  but settled on "varchar" to maintain parity with "varbinary" (below).]
                new KeyValuePair<string,Type>(
                    "varchar"
                    ,typeof(String)
                )
                ,new KeyValuePair<string,Type>(
                    "int32"
                    ,typeof(Int32)
                )
                ,new KeyValuePair<string,Type>(
                    "int64"
                    ,typeof(Int64)
                )
                ,new KeyValuePair<string,Type>(
                    "float32"
                    ,typeof(Single)
                )
                ,new KeyValuePair<string,Type>(
                    "float64"
                    ,typeof(Double)
                )
                ,new KeyValuePair<string,Type>(
                    "decimal"
                    ,typeof(Decimal)
                )
                ,new KeyValuePair<string,Type>(
                    "boolean"
                    ,typeof(Boolean)
                )
                // [20170322 [db] Chose "varbinary" since it seemed like the most conventional
                //  term (from sql server anyway) for this.  
                //  Postgresql's "bytea" doesn't seem very intuitive,
                //  "byte_array" is too clumsy, "bytes" is unconventional, 
                //  and "blob" is probably misleading]
                ,new KeyValuePair<string,Type>(
                    "varbinary"
                    ,typeof(byte[])
                )
                ,new KeyValuePair<string,Type>(
                    "guid"
                    ,typeof(Guid)
                )
                ,new KeyValuePair<string,Type>(
                    "datetime"
                    ,typeof(DateTime)
                )
                ,new KeyValuePair<string,Type>(
                    "timespan"
                    ,typeof(TimeSpan)
                )
            };

            foreach(KeyValuePair<string,Type> dataTypeInfo in dataTypeList)
            {
                string dataTypeName = dataTypeInfo.Key;
                Type dataType = dataTypeInfo.Value;

                dataTypeDictionary[dataTypeName] = dataType;
                dataTypeNameDictionary[dataType] = dataTypeName;
            }

        }

        /// <summary>
        /// Try to find the type name used for this field schema
        /// that corresponds to some .NET datatype
        /// </summary>
        /// <param name="dataType">.NET datatype search criterion</param>
        /// <param name="defaultFieldTypeName">field type name to use if no field type name is found</param>
        /// <returns>a field type name, if the .net datatype was found, then 
        /// this will be the primary name assigned to that type by this field spec,
        /// if the .net datatype was not found, then this will be the same as the <c>defaultFieldTypeName</c> parameter.
        /// </returns>
        public string
        GetFieldTypeName(
            Type dataType
            ,string defaultFieldTypeName
            )
        {
            bool wasFound = false;
            string fieldTypeName = null;

            if (null != dataType)
            {
                wasFound = _dataTypeNameDictionary.TryGetValue(dataType, out fieldTypeName);
            }
            if (!wasFound)
            {
                fieldTypeName = defaultFieldTypeName;
            }

            return fieldTypeName;
        }

        /// <summary>
        /// Centralized function to lookup a datatype from its name
        /// </summary>
        /// <param name="fieldTypeName">a type name used by the field spec</param>
        /// <param name="defaultDataType">a .NET datatype that will be used as a default if
        /// no corresponding .NET datatype could be found for the field type name
        /// </param>
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
        /// Format a field spec from a field name and a field type
        /// </summary>
        /// <param name="fieldName">name of the record field to encode</param>
        /// <param name="fieldType">name of the field type</param>
        /// <returns>a "field spec" which encodes a field name and its type into a string</returns>
        public string
        EncodeField(
            string fieldName
            ,IRecordFieldType<TValue> fieldType
            )
        {
            string fieldSpecString = fieldName;
            string fieldTypeName = null;

            FieldSchemaSpecFieldType<TValue> fieldSchemaSpecType = fieldType as FieldSchemaSpecFieldType<TValue>;
            if (null != fieldSchemaSpecType)
            {
                fieldTypeName = fieldSchemaSpecType.FieldTypeName;
            }
            else if (null != fieldType)
            {
                string unknownFieldTypeName = null;
                fieldTypeName = GetFieldTypeName(fieldType.DataType, unknownFieldTypeName);
            }

            if (!String.IsNullOrEmpty(fieldName)
                && null != fieldTypeName
                )
            {
                fieldSpecString = String.Format("{0}:{1}"
                        ,fieldName
                        ,fieldTypeName
                        );
            }

            return fieldSpecString;
        }

        /// <summary>
        /// Parse a field schema spec into an enumeration of record field type objects
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string,FieldSchemaSpecFieldType<TValue>>> 
        DecodeEnumerable(
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
        public KeyValuePair<string,FieldSchemaSpecFieldType<TValue>>
        DecodeField(
            string fieldSchemaSpecString
            ,int startPosition = 0
            ,StringBuilder internalBuffer = null
            )
        {
            StringBuilder buffer = internalBuffer;
            string fieldName;
            FieldSchemaSpecFieldType<TValue> fieldType;
            
            if (null == buffer)
            {
                buffer = new StringBuilder();
            }
            else
            {
                internalBuffer.Clear();
            }

            DecodeField(
                fieldSchemaSpecString
                ,startPosition
                ,out fieldName
                ,out fieldType
                ,buffer
                );
            
            return new KeyValuePair<string,FieldSchemaSpecFieldType<TValue>>(fieldName,fieldType);
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
        DecodeField(
             string fieldSchemaSpecString
            ,int startPosition
            ,out string fieldName
            ,out FieldSchemaSpecFieldType<TValue> fieldType
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

            if (null != specString
                && 0 < specString.Length
                )
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
                        fieldType = new FieldSchemaSpecFieldType<TValue>(
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
        : IEnumerable<KeyValuePair<string,FieldSchemaSpecFieldType<TValue>>>
        {
            private readonly FieldSchemaSpecEncoding<TValue> _encoding;
            private string _fieldSchemaSpecString;

            internal FieldSchemaSpecEnumeration(
                 FieldSchemaSpecEncoding<TValue> encoding
                ,string fieldSchemaSpecString
                )
            {
                _encoding = encoding;
                _fieldSchemaSpecString = fieldSchemaSpecString;
            }

            public IEnumerator<KeyValuePair<string,FieldSchemaSpecFieldType<TValue>>>
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
                        FieldSchemaSpecFieldType<TValue> fieldType;

                        charCount = _encoding.DecodeField(
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
                            yield return new KeyValuePair<string,FieldSchemaSpecFieldType<TValue>>(fieldName, fieldType);
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
