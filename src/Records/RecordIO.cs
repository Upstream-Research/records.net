/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

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
        /// Copy the fields from a record into a target record
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="targetRecord"></param>
        /// <param name="record"></param>
        public static void 
        CopyInto<TValue>(
            IRecordAccessor<TValue> targetRecord
            ,IRecordAccessor<TValue> record
            )
        {
            if (null != targetRecord
                && null != record
                )
            {
                IEnumerator<string> fieldNameEnumerator = targetRecord.GetFieldNameEnumerator();
                int fieldPosition = 0;
                while (fieldNameEnumerator.MoveNext())
                {
                    string fieldName = fieldNameEnumerator.Current;
                    TValue fieldValue;
                    if (record.TryGetValue(fieldName, out fieldValue))
                    {
                        // set using fieldPosition instead of fieldName
                        //  since fieldPosition is typically faster
                        targetRecord[fieldPosition] = fieldValue;
                    }

                    fieldPosition += 1;
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
        public long ReadInto<TValue>(
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

    } // /class

} // /class
