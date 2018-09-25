/*  Copyright (c) 2018 Upstream Research, Inc.  */
/*  Subject to an MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Upstream.System.Records
{
    /// <summary>
    /// Implements a record adapter that provides access to a "selection" of fields from another record schema.
    /// </summary>
    /// <typeparam name="TValue">value type for record fields</typeparam>
    /// <typeparam name="TFieldType">field type for record schema</typeparam>
    public class FieldSelectionRecordAccessorAdapter<TValue, TFieldType>
    : IRecordAccessorAdapter<TValue, IRecordAccessor<TValue>>
    {
        readonly IList<int> _selectedFieldPositionList;
        readonly BasicRecordSchema<TFieldType> _selectedRecordSchema;
        readonly IRecordSchemaViewer<TFieldType> _baseRecordSchema;
        readonly TFieldType _defaultFieldType;
        readonly TValue _defaultFieldValue;
        
        IRecordAccessor<TValue> _baseRecord;

        /// <summary>
        /// Create a field selection record accessor over a base records schema.
        /// The selected fields must be added to the adapter using the <c>AddField()</c> method.
        /// The adapter must be attached to a base record using the <c>AttachTo()</c> method.
        /// </summary>
        /// <param name="baseRecordSchema">Record schema for the source records</param>
        /// <param name="defaultFieldType">Field type to use if a selected field is not found on the base schema</param>
        /// <param name="defaultFieldValue">Value to assign to fields which are not present in the base schema</param>
        public FieldSelectionRecordAccessorAdapter(
            IRecordSchemaViewer<TFieldType> baseRecordSchema
            ,TFieldType defaultFieldType
            ,TValue defaultFieldValue = default(TValue)
            )
        {
            if (null == baseRecordSchema)
            { 
                throw new ArgumentNullException("baseRecordSchema");
            }

            int estimatedFieldCount = baseRecordSchema.GetFieldCount();
            _selectedFieldPositionList = new List<int>(estimatedFieldCount);
            _selectedRecordSchema = new BasicRecordSchema<TFieldType>();
            _baseRecordSchema = baseRecordSchema;
            _defaultFieldType = defaultFieldType;
            _defaultFieldValue = defaultFieldValue;
        }

        /// <summary>
        /// Attach this record accessor to a record
        /// </summary>
        /// <param name="baseRecord">source record object</param>
        public void AttachTo(IRecordAccessor<TValue> baseRecord)
        {
            _baseRecord = baseRecord;
        }

        /// <summary>
        /// Add a new field to the field selection which will get its value from a "base" field on the attached record
        /// </summary>
        /// <param name="fieldName">name of the field to be exposed by this record.</param>
        /// <param name="baseFieldName">
        /// name of the field on the base record which will supply the field value for this field.
        /// If this is a null reference, then the adapted record will always return a default value for this field.
        /// </param>
        public void AddField(
             string fieldName
            ,string baseFieldName
            )
        {
            TFieldType fieldType = _defaultFieldType;
            int baseFieldPosition = -1;
            
            if (null == fieldName)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (null != baseFieldName)
            {
                baseFieldPosition = _baseRecordSchema.IndexOfField(baseFieldName);
            }
            
            if (0 <= baseFieldPosition
                && baseFieldPosition < _baseRecordSchema.GetFieldCount()
                )
            {
                fieldType = _baseRecordSchema[baseFieldPosition];
            }

            _selectedFieldPositionList.Add(baseFieldPosition);
            _selectedRecordSchema.AddField(
                fieldName
                ,fieldType
                );
        }

        /// <summary>
        /// Get the record schema for this adapted record
        /// </summary>
        public IRecordSchemaViewer<TFieldType> RecordSchema
        {
            get
            {
                return _selectedRecordSchema;
            }
        }

        /// <summary>
        /// Get the record schema for the base record
        /// </summary>
        public IRecordSchemaViewer<TFieldType> BaseRecordSchema
        {
            get
            {
                return _baseRecordSchema;
            }
        }

        /// <summary>
        /// Get the record to which this record is attached
        /// </summary>
        public IRecordAccessor<TValue> BaseRecord
        {
            get
            {
                return _baseRecord;
            }
        }

        /// <summary>
        /// Get/Set a field value by its position in the record
        /// </summary>
        /// <param name="fieldPosition"></param>
        /// <returns></returns>
        public TValue this[int fieldPosition]
        {
            get
            {
                if (null == BaseRecord)
                {
                    throw new InvalidOperationException("Record adapter is not attached to a record");
                }
                int fieldCount = _selectedFieldPositionList.Count;
                if (0 > fieldPosition
                    || fieldCount <= fieldPosition
                    )
                {
                    throw new ArgumentOutOfRangeException("fieldPosition");
                }
                int baseFieldCount = BaseRecord.GetFieldCount();
                int baseFieldPosition = _selectedFieldPositionList[fieldPosition];
                TValue fieldValue = _defaultFieldValue;
                if (0 <= baseFieldPosition
                    && baseFieldPosition < baseFieldCount
                    )
                {
                    fieldValue = BaseRecord[baseFieldPosition];
                }

                return fieldValue;
            }

            set
            {
                TValue fieldValue = value;

                if (null == BaseRecord)
                {
                    throw new InvalidOperationException("Record adapter is not attached to a record");
                }
                int fieldCount = _selectedFieldPositionList.Count;
                if (0 > fieldPosition
                    || fieldCount <= fieldPosition
                    )
                {
                    throw new ArgumentOutOfRangeException("fieldPosition");
                }
                int baseFieldCount = BaseRecord.GetFieldCount();
                int baseFieldPosition = _selectedFieldPositionList[fieldPosition];
                if (0 <= baseFieldPosition
                    && baseFieldPosition < baseFieldCount
                    )
                {
                    BaseRecord[baseFieldPosition] = fieldValue;
                }
            }
        }

        /// <summary>
        /// Get/Set a field value by its field name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public TValue this[string fieldName]
        {
            get
            {
                if (null == BaseRecord)
                {
                    throw new InvalidOperationException("Record adapter is not attached to a record");
                }

                int fieldPosition = RecordSchema.IndexOfField(fieldName);
                int fieldCount = _selectedFieldPositionList.Count;
                if (0 > fieldPosition
                    || fieldCount <= fieldPosition
                    )
                {
                    throw new KeyNotFoundException(String.Format("Field not found: '{0}'", fieldName));
                }
                int baseFieldCount = BaseRecord.GetFieldCount();
                int baseFieldPosition = _selectedFieldPositionList[fieldPosition];
                TValue fieldValue = _defaultFieldValue;
                if (0 <= baseFieldPosition
                    && baseFieldPosition < baseFieldCount
                    )
                {
                    fieldValue = BaseRecord[baseFieldPosition];
                }

                return fieldValue;
            }

            set
            {
                TValue fieldValue = value;

                if (null == BaseRecord)
                {
                    throw new InvalidOperationException("Record adapter is not attached to a record");
                }

                int fieldPosition = RecordSchema.IndexOfField(fieldName);
                int fieldCount = _selectedFieldPositionList.Count;
                if (0 > fieldPosition
                    || fieldCount <= fieldPosition
                    )
                {
                    throw new KeyNotFoundException(String.Format("Field not found: '{0}'", fieldName));
                }
                int baseFieldCount = BaseRecord.GetFieldCount();
                int baseFieldPosition = _selectedFieldPositionList[fieldPosition];
                if (0 <= baseFieldPosition
                    && baseFieldPosition < baseFieldCount
                    )
                {
                    BaseRecord[baseFieldPosition] = fieldValue;
                }
            }
        }

        TValue IRecordViewer<TValue>.this[int fieldPosition]
        {
            get
            {
                return this[fieldPosition];
            }
        }

        TValue IRecordViewer<TValue>.this[string fieldName]
        {
            get
            {
                return this[fieldName];
            }
        }

        /// <summary>
        /// Get the number of selected fields
        /// </summary>
        /// <returns></returns>
        public int GetFieldCount()
        {
            return RecordSchema.GetFieldCount();
        }

        /// <summary>
        /// Get the field position for a field
        /// </summary>
        /// <param name="fieldName">name of field</param>
        /// <returns>-1 if field is not found</returns>
        public int IndexOfField(string fieldName)
        {
            return RecordSchema.IndexOfField(fieldName);
        }

        /// <summary>
        /// Try to get the value of a field by its name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="outValue"></param>
        /// <returns>true if the field was found</returns>
        public bool TryGetValue(string fieldName, out TValue outValue)
        {
            bool wasFound = false;
            int fieldPosition = RecordSchema.IndexOfField(fieldName);
            int fieldCount = _selectedFieldPositionList.Count;

            outValue = default(TValue);
            if (null != BaseRecord
                && 0 <= fieldPosition
                && fieldCount > fieldPosition
            )
            {
                int baseFieldPosition = _selectedFieldPositionList[fieldPosition];
                int baseFieldCount = BaseRecord.GetFieldCount();
                if (0 <= baseFieldPosition
                    && baseFieldPosition < baseFieldCount
                    )
                {
                    outValue = BaseRecord[baseFieldPosition];
                }
                else
                {
                    outValue = _defaultFieldValue;
                }
                // even if the field does not exist on the base record,
                //  we have still "found" it, since it exists in our schema mapping
                wasFound = true;
            }

            return wasFound;
        }

        /// <summary>
        /// Get an enumerator of field names and values
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IFieldNameValuePair<TValue>> GetEnumerator()
        {
            if (null == BaseRecord)
            {
                throw new InvalidOperationException("Record adapter is not attached to a record");
            }
            int fieldPosition;
            int fieldCount = RecordSchema.GetFieldCount();
            int baseFieldCount = BaseRecord.GetFieldCount();

            for (fieldPosition = 0;
                fieldPosition < fieldCount
                && fieldPosition < _selectedFieldPositionList.Count
                ;fieldPosition++
                )
            {
                string fieldName = RecordSchema.FieldNameAt(fieldPosition);
                TValue fieldValue = _defaultFieldValue;
                int baseFieldPosition = _selectedFieldPositionList[fieldPosition];
                if (0 <= baseFieldPosition
                    && baseFieldCount > baseFieldPosition
                    )
                {
                    fieldValue = BaseRecord[baseFieldPosition];
                }
                yield return new FieldNameValuePair<TValue>(fieldName, fieldValue);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    } // /class

} // /namespace
