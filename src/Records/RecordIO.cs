/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides some helper methods for copying data between records
    /// </summary>
    /// <remarks>
    /// [20170324 [db] It is arguable that the helper functions used here should
    ///  be implemented as extension methods.  Eventually, this may be done, 
    ///  but presently, I am rather displeased with extension methods
    ///  and prefer to avoid using them.]
    /// </remarks>
    public class RecordIO
    {
        /// <summary>
        /// Create a "mapping" that can be used to quickly match fields from records in two different schema.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="targetRecord"></param>
        /// <param name="sourceRecord"></param>
        /// <returns>An object that can be passed to the CopyInto() method</returns>
        public static IEnumerable<int> 
        CreateFieldPositionMapping<TValue>(
            IRecordAccessor<TValue> targetRecord
            ,IRecordViewer<TValue> sourceRecord
            )
        {
            // sourcePositionArray will be an array that contains the positions from the source record,
            //  it will have one item for each field in the target record.
            int[] sourcePositionArray = null;

            if (null != targetRecord    
                && null != sourceRecord
                )
            {
                int sourceFieldCount = sourceRecord.GetFieldCount();
                string[] sourceFieldNameArray = new string[sourceFieldCount];
                int sourceFieldPosition = 0;
                foreach(IFieldNameValuePair<TValue> sourceFieldItem in sourceRecord)
                {
                    string sourceFieldName = sourceFieldItem.Name;
                    sourceFieldNameArray[sourceFieldPosition] = sourceFieldName;
                    sourceFieldPosition += 1;
                }

                int targetFieldCount = targetRecord.GetFieldCount();
                sourcePositionArray = new int[targetFieldCount];
                int targetFieldPosition = 0;
                foreach (IFieldNameValuePair<TValue> targetFieldItem in targetRecord)
                {
                    string targetFieldName = targetFieldItem.Name;
                    int foundSourceFieldPosition = -1;
                    sourceFieldPosition = 0;
                    while (0 > foundSourceFieldPosition
                        && sourceFieldNameArray.Length > sourceFieldPosition
                        )
                    {
                        string sourceFieldName = sourceFieldNameArray[sourceFieldPosition];
                        if (FieldNamesAreEqual(sourceFieldName, targetFieldName))
                        {
                            foundSourceFieldPosition = sourceFieldPosition;
                        }
                        sourceFieldPosition += 1;
                    }
                    sourcePositionArray[targetFieldPosition] = foundSourceFieldPosition;
                    targetFieldPosition += 1;
                }

            }

            return sourcePositionArray;
        }

        /// <summary>
        /// Copy the fields from a record into a target record
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="targetRecord"></param>
        /// <param name="record"></param>
        /// <param name="fieldPositionMapping">
        /// An enumeration of field positions for each field in the target record,
        /// fields that do not map into the target record will have a position of -1.
        /// </param>
        public static void 
        CopyInto<TValue>(
            IRecordAccessor<TValue> targetRecord
            ,IRecordViewer<TValue> record
            ,IEnumerable<int> fieldPositionMapping = null
            )
        {
            if (null == fieldPositionMapping)
            {
                fieldPositionMapping = CreateFieldPositionMapping(targetRecord, record);
            }
            if (null != targetRecord
                && null != record
                )
            {
                int fieldCount = record.GetFieldCount();
                int targetFieldPosition = 0;
                foreach (int fieldPosition in fieldPositionMapping)
                {
                    if (0 <= fieldPosition
                        && fieldPosition < fieldCount
                        )
                    {
                        targetRecord[targetFieldPosition] = record[fieldPosition];
                    }
                    targetFieldPosition += 1;
                }
            }
        }

        /// <summary>
        /// Read records from a record enumerator into a record collector
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collector"></param>
        /// <param name="enumerator"></param>
        /// <returns>number of record read</returns>
        public static long 
        ReadInto<TValue>(
            IRecordCollector<TValue> collector
            ,IRecordEnumerator<TValue> enumerator
            )
        {
            long recordCount = 0;

            if (null != collector
                && null != enumerator
                )
            {
                while (enumerator.MoveNext()
                    && collector.Add(enumerator.Current)
                    )
                {
                    recordCount += 1;
                }
            }

            return recordCount;
        }

        /// <summary>
        /// Determine if two field names are equivalent
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldName2"></param>
        /// <returns></returns>
        private static bool
        FieldNamesAreEqual(string fieldName, string fieldName2)
        {
            return (0 == String.Compare(fieldName, fieldName2, StringComparison.Ordinal));
        }
    } // /class

} // /class
