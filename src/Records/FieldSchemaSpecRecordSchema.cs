/*  Copyright (c) 2018 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements IRecordSchemaAccessor for a field schema spec
    /// </summary>
    public class FieldSchemaSpecRecordSchema<TValue>
    : IRecordSchemaAccessor<FieldSchemaSpecFieldType<TValue>>
    {
        private readonly BasicRecordSchema<FieldSchemaSpecFieldType<TValue>> _baseRecordSchema;

        private BasicRecordSchema<FieldSchemaSpecFieldType<TValue>> BaseRecordSchema
        {
            get
            {
                return _baseRecordSchema;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public FieldSchemaSpecFieldType<TValue> this[int fieldPosition]
        {
            get
            {
                return BaseRecordSchema[fieldPosition];
            }

            set
            {
                BaseRecordSchema[fieldPosition] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldSchemaSpecFieldType<TValue> this[string fieldName]
        {
            get
            {
                return BaseRecordSchema[fieldName];
            }

            set
            {
                BaseRecordSchema[fieldName] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public bool TryGetValue(string fieldName, out FieldSchemaSpecFieldType<TValue> outValue)
        {
            return BaseRecordSchema.TryGetValue(fieldName, out outValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> FieldNames
        {
            get
            {
                return BaseRecordSchema.FieldNames;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return BaseRecordSchema.GetFieldCount();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public string FieldNameAt(int fieldPosition)
        {
            return BaseRecordSchema.FieldNameAt(fieldPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int IndexOfField(string fieldName)
        {
            return BaseRecordSchema.IndexOfField(fieldName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> 
        GetFieldNameEnumerator()
        {
            return BaseRecordSchema.GetFieldNameEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, FieldSchemaSpecFieldType<TValue>>> 
        GetEnumerator()
        {
            return BaseRecordSchema.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator 
        IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // /class

} // /namespace
