/*  Copyright (c) 2015 Upstream Research, Inc.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Text;

namespace Upstream.System.Csv
{
    /// <summary>
    /// Basic Code Separated Value (CSV) encoder/decoder object
    /// </summary>
    /// <remarks>
    /// "Code Separated Value" is supposed to be a generalization of "Comma Separated Value"
    /// with a conveniently identical abbreviation.
    /// The terms "Record Separator" and "Unit Separator" refer to the ASCII control codes (0x30 and 0x31 respectively) 
    /// which were originally defined for the purpose of encoding CSV-type data.
    /// The Wikipedia-sanctioned term is "Separator-Separated-Values".
    /// </remarks>
    public 
    class CsvEncoding
    {
        private const string DefaultUnitSeparator = ",";        
        private const string DefaultQuote = "\"";
    
        private StringComparison _codeComparisonOptions;
        private string _recordSeparator;
        private string _unitSeparator;
        private string _quote;
        private string _quoteEscape;
        private bool _shouldQuoteEmptyStringValues;
        
        /// <summary>
        /// Create a CSV Encoding object which can be used for encoding and decoding CSV data
        /// </summary>
        /// <param name="unitSeparator">separator code used between value units, defaults to a comma (',')</param>
        /// <param name="quote">quotation symbol used to escape the separator codes, defaults to a double-quote ('"')</param>
        /// <param name="quoteEscape">quotation escape symbol, defaults to a double quote symbol (e.g. '""')</param>
        /// <param name="recordSeparator">separator between records, defaults to the local platform's newline character</param>
        /// <param name="shouldQuoteEmptyStringValues">True if empty strings should be encoded by quoting them
        /// (an null string will not be quoted when it is encoded)</param>
        public 
        CsvEncoding(
             string unitSeparator = null
            ,string quote = null
            ,string quoteEscape = null
            ,string recordSeparator = null
            ,bool shouldQuoteEmptyStringValues = false
            )
        {
            _codeComparisonOptions = StringComparison.Ordinal;
            _recordSeparator = recordSeparator;
            _unitSeparator = unitSeparator;
            _quote = quote;
            _quoteEscape = quoteEscape;
            _shouldQuoteEmptyStringValues = shouldQuoteEmptyStringValues;
            
            if (null == _unitSeparator)
            {
                _unitSeparator = DefaultUnitSeparator;
            }
            
            if (0 == _unitSeparator.Length)
            {
                throw new ArgumentException("unitSeparator cannot be an empty string", "unitSeparator");
            }
            
            if (null == _quote)
            {
                _quote = DefaultQuote;
            }
            
            if (null == _quoteEscape)
            {
                _quoteEscape = _quote + _quote;
            }
            
            if (null == _recordSeparator)
            {
                // a null or empty row Separator will signal the use of a default newline sequence depending on the context
                _recordSeparator = String.Empty;
            }
        }
                
        /// <summary>
        /// Get the value separator character string
        /// </summary>
        /// <returns>unit/value separator code string</returns>
        public string UnitSeparator
        {
            get
            {
                return _unitSeparator;
            }
        }
        
        /// <summary>
        /// Get the quote character which can be used to escape the unit and record separators
        /// </summary>
        /// <returns>unit/value quotation code</returns>
        public string Quote
        {
            get
            {
                return _quote;
            }
        }
        
        /// <summary>
        /// Get the quote character escape string that can be used to escape the quote sequence 
        /// inside of a quoted unit
        /// </summary>
        /// <returns>unit/value quotation code escape code</returns>
        public string QuoteEscape
        {
            get
            {
                return _quoteEscape;
            }
        }

        /// <summary>
        /// Get the symbol used for separating records
        /// </summary>
        /// <returns>record separator code string</returns>
        public string RecordSeparator
        {
            get
            {
                string defaultRecordSeparator = Environment.NewLine;
                string recordSeparator = _recordSeparator;
            
                if (null == recordSeparator 
                    || 0 >= recordSeparator.Length
                    )
                {
                    recordSeparator = defaultRecordSeparator;
                }
            
                return recordSeparator;
            }
        }

        /// <summary>
        /// True if empty strings should be quoted when they are encoded
        /// </summary>
        public bool ShouldQuoteEmptyStrings
        {
            get
            {
                return _shouldQuoteEmptyStringValues;
            }
        }

        /// <summary>
        /// Get the StringComparison type used for comparing separator codes
        /// </summary>
        private StringComparison CodeComparisonOptions
        {
            get
            {
                return _codeComparisonOptions;
            }
        }

        /// <summary>
        /// Find the first occurrence of a code within a search string
        /// </summary>
        /// <param name="sourceString">string to search</param>
        /// <param name="codeString">string "code" to search for</param>
        /// <param name="sourceStringStartIndex">position in the source string where searching will begin</param>
        /// <returns>-1 if the code was not found</returns>
        private int
        IndexOfCode(
             string sourceString
            ,string codeString
            ,int sourceStringStartIndex = 0
            )
        {
            int foundIndex = -1;
            if (null != sourceString
                && null != codeString
                && 0 < codeString.Length
                )
            {
                foundIndex = sourceString.IndexOf(codeString, sourceStringStartIndex, CodeComparisonOptions);
            }

            return foundIndex;
        }
        
        /// <summary>
        /// Check if a code exists at a position within a target string
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="codeString"></param>
        /// <param name="sourceStringStartIndex"></param>
        /// <returns></returns>
        private bool
        EqualsCode(
             string sourceString
            ,string codeString
            ,int sourceStringStartIndex = 0
            )
        {
            bool sourceStartsWithCode = false;

            if (null != sourceString
                && null != codeString
                && 0 < codeString.Length
                && 0 <= sourceStringStartIndex 
                && sourceString.Length > sourceStringStartIndex
                && codeString.Length <= (sourceString.Length - sourceStringStartIndex)
                )
            {
                string targetPrefix = sourceString.Substring(sourceStringStartIndex, codeString.Length);
                if (0 == String.Compare(codeString, targetPrefix, CodeComparisonOptions))
                {
                    sourceStartsWithCode = true;
                }
            }

            return sourceStartsWithCode;
        }

        /// <summary>
        /// Check if the record separator appears in a string
        /// </summary>
        /// <param name="sourceString">string to search</param>
        /// <param name="sourceStringStartIndex">position in the string to start searching</param>
        /// <returns></returns>
        public bool
        EqualsRecordSeparator(
             string sourceString
            ,int sourceStringStartIndex
            )
        {
            return EqualsCode(sourceString, RecordSeparator, sourceStringStartIndex);
        }

        /// <summary>
        /// Check if the unit separator appears in the target string
        /// </summary>
        /// <param name="sourceString">string to search</param>
        /// <param name="sourceStringStartIndex">position in the string to start searching</param>
        /// <returns></returns>
        public bool
        EqualsUnitSeparator(
            string sourceString
            ,int sourceStringStartIndex
            )
        {
            return EqualsCode(sourceString, UnitSeparator, sourceStringStartIndex);
        }

        /// <summary>
        /// Decode a single unit value from an encoded record
        /// </summary>
        /// <param name="decodedValueBuffer">target buffer to receive the decoded value</param>
        /// <param name="isInsideQuotation">
        /// Should be initially set to False.
        /// Stores the state of the decoder;
        /// will be set to true if a quote is started but left unfinished.
        /// </param>
        /// <param name="encodedTextOffset">
        /// on entry, the offset in the encodedText string where decoding should begin.
        /// on exit, the offset of the next place in the encodedText string where decoding should continue.
        /// </param>
        /// <param name="encodedText">encoded text to read</param>
        /// <returns>
        /// Number of characters read from the encodedText string,
        /// this will include quotes, but will not include the terminating separator (if there is one).
        /// </returns>
        public int
        DecodeInto(
             StringBuilder decodedValueBuffer
            ,ref bool isInsideQuotation
            ,ref int encodedTextOffset
            ,string encodedText
            )
        {
            string recordSeparator = RecordSeparator;
            string unitSeparator = UnitSeparator;
            string quote = Quote;
            string quoteEscape = QuoteEscape;
            int encodedTextReadCount = 0;
            //int encodedTextInitialOffset = encodedTextOffset;
            int separatorOffset = -1;
            
            if (null == decodedValueBuffer)
            {
                throw new ArgumentNullException("decodedValueBuffer");
            }
            if (0 > encodedTextOffset)
            {
                throw new ArgumentOutOfRangeException("encodedTextOffset");
            }
            if (null == encodedText)
            {
                return encodedTextReadCount;
            }
            
            while (encodedTextOffset < encodedText.Length
                && 0 > separatorOffset
                )
            {
                int symbolLength = 0;

                if (isInsideQuotation)
                {
                    if (EqualsCode(encodedText, QuoteEscape, encodedTextOffset))
                    {
                        decodedValueBuffer.Append(Quote);
                        symbolLength = QuoteEscape.Length;
                    }
                    else if (EqualsCode(encodedText, Quote, encodedTextOffset))
                    {
                        isInsideQuotation = false;
                        symbolLength = Quote.Length;
                    }
                }
                else
                {
                    if (EqualsCode(encodedText, Quote, encodedTextOffset))
                    {
                        isInsideQuotation = true;
                        symbolLength = Quote.Length;
                    }
                    else if (EqualsCode(encodedText, RecordSeparator, encodedTextOffset))
                    {
                        symbolLength = RecordSeparator.Length;
                        separatorOffset = encodedTextOffset;
                    }
                    else if (EqualsCode(encodedText, UnitSeparator, encodedTextOffset))
                    {
                        symbolLength = UnitSeparator.Length;
                        separatorOffset = encodedTextOffset;
                    }
                }
                if (0 == symbolLength)
                {
                    decodedValueBuffer.Append(encodedText[encodedTextOffset]);
                    symbolLength = 1;
                }

                encodedTextOffset += symbolLength;
                if (0 > separatorOffset)
                {
                    encodedTextReadCount += symbolLength;
                }
            }

            return encodedTextReadCount;
        }

        /// <summary>
        /// Append a unit value onto a string buffer.
        /// The value will be quoted if necessary
        /// </summary>
        /// <param name="encodedValueBuffer">target buffer to receive the encoded value</param>
        /// <param name="valueString">string value to encode</param>
        /// <returns>The number of characters written to the buffer.
        /// This will be zero if the value string is null (or the equivalent)
        /// </returns>
        public int
        EncodeValueInto(
             StringBuilder encodedValueBuffer
            ,string valueString
            )
        {
            int encodedTextWriteCount = 0;
            int valueStringStartIndex = 0;
            bool shouldQuoteValue = false;

            if (null == encodedValueBuffer)
            {
                throw new ArgumentNullException("encodedValueBuffer");
            }
            if (null != valueString)
            {
                if (0 == valueString.Length
                    && ShouldQuoteEmptyStrings
                    )
                {
                    shouldQuoteValue = true;
                }

                // search for code symbols to see if we will need to quote
                int valueStringIndex = valueStringStartIndex;
                int symbolIndex = -1;
                while (valueStringIndex < valueString.Length
                    && 0 > symbolIndex
                    )
                {
                    if (EqualsCode(valueString, Quote, valueStringIndex)
                        || EqualsCode(valueString, UnitSeparator, valueStringIndex)
                        || EqualsCode(valueString, RecordSeparator, valueStringIndex)
                        )
                    {
                        symbolIndex = valueStringIndex;
                    }
                    
                    valueStringIndex++;
                }
                if (0 <= symbolIndex)
                {
                    shouldQuoteValue = true;
                }
                if (shouldQuoteValue)
                {
                    encodedValueBuffer.Append(Quote);
                    encodedTextWriteCount += Quote.Length;
                }

                // encode one symbol at a time
                valueStringIndex = valueStringStartIndex;
                while (valueStringIndex < valueString.Length)
                {
                    int symbolLength = 0;

                    if (shouldQuoteValue
                        && EqualsCode(valueString, Quote, valueStringIndex)
                        )
                    {
                        encodedValueBuffer.Append(QuoteEscape);
                        symbolLength += Quote.Length;
                    }
                    if (0 == symbolLength)
                    {
                        encodedValueBuffer.Append(valueString[valueStringIndex]);
                        symbolLength += 1;
                    }
                    valueStringIndex += symbolLength;
                    encodedTextWriteCount += Quote.Length;
                }
                if (shouldQuoteValue)
                {
                    encodedValueBuffer.Append(Quote);
                    encodedTextWriteCount += Quote.Length;
                }
            }

            return encodedTextWriteCount;
        }

    } // /class
    
} // /namespace
