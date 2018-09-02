/* Copyright (c) 2015-2017 Upstream Research, Inc. */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
//using System.Runtime.Serialization;

namespace Upstream.System.Csv
{
    /// <summary>
    /// Basic Exception for CSV components
    /// </summary>
    public class CsvException
    : Exception
    {
        /// <summary>
        /// Create a new, empty CSV exception
        /// </summary>
        public CsvException()
            : base()
        {
        }
        
        /// <summary>
        /// Create a new CSV exception with an error message
        /// </summary>
        /// <param name="message"></param>
        public CsvException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Create a new CSV exception that will wrap another exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CsvException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        /*
        /// <summary>
        /// Create a new CSV exception from serialized information
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        public CsvException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
        */
    } // /class
    
} // /namespace
