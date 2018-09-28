/*  Copyright (c) 2018 Upstream Research, Inc.  */
/*  Subject to an MIT license.  See LICENSE file in top-level directory.  */

using System;
using System.Collections.Generic;

using Upstream.System.Csv;
using Upstream.System.Records;

namespace Upstream.System.Records.Csv
{
    /// <summary>
    /// Utility class containing static methods implementing 
    /// some high-level CSV/Record operations.
    /// </summary>
    public sealed class CsvRecordIO
    {
        /// <summary>
        /// Private constructor to prevent instantiation
        /// </summary>
        private CsvRecordIO()
        {
        }

        /// <summary>
        /// Create a record enumerator on a CSV input stream.
        /// The record schema will be read from the header row of the CSV stream,
        /// and all fields will be assumed to be of the same, string-based datatype.
        /// </summary>
        /// <param name="csvReader"></param>
        /// <returns>
        /// A record enumerator that will read records from a CSV stream.
        /// Disposing of the enumerator will also dispose of the underlying CSV reader.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This implements a very basic record enumerator that will only create records
        /// containing string values.
        /// </para>
        /// </remarks>
        public static IRecordEnumerator<string>
        CreateRecordEnumerator(
            CsvReader csvReader
            )
        {
            if (null == csvReader)
            {
                return null;
            }

            StringComparer valueComparer = StringComparer.Ordinal;
            IComparer<string> sortComparer = valueComparer;
            IEqualityComparer<string> eqComparer = valueComparer;
            IList<string> fieldNameList = new List<string>();

            if (csvReader.ReadRecord())
            {
                while (csvReader.ReadValue())
                {
                    string fieldName = csvReader.ValueText;
                    fieldNameList.Add(fieldName);
                }
            }

            return new CsvRecordEnumerator(
                 csvReader
                ,fieldNameList
                ,sortComparer
                ,eqComparer
            );
        }

        /// <summary>
        /// Create a new record collection builder over a CSV stream.
        /// The field names will be written to the CSV stream in a header row
        /// before writing the CSV records.
        /// </summary>
        /// <param name="csvWriter"></param>
        /// <param name="fieldNameEnumeration"></param>
        /// <returns>
        /// A record collection builder that will write records to a CSV stream.
        /// Disposing of the collection builder will dispose of the underlying CSV writer.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This implements a very basic record collector that will only accept records
        /// containing string values.
        /// </para>
        /// </remarks>
        public static IRecordCollectionBuilder<string>
        CreateCsvRecordCollectionBuilder(
            CsvWriter csvWriter
            ,IEnumerable<string> fieldNameEnumeration
            )
        {
            if (null == csvWriter)
            {
                return null;
            }

            StringComparer valueComparer = StringComparer.Ordinal;
            IComparer<string> sortComparer = valueComparer;
            IEqualityComparer<string> eqComparer = valueComparer;

            if (null != fieldNameEnumeration)
            {
                csvWriter.WriteStartRecord();
                foreach (string fieldName in fieldNameEnumeration)
                {
                    csvWriter.WriteValue(fieldName);
                }
                csvWriter.WriteEndRecord();
            }

            return new CsvRecordCollectionBuilder(
                csvWriter
                ,fieldNameEnumeration
                ,sortComparer
                ,eqComparer
                );
        }
        

    } // /class

} // /namespace
