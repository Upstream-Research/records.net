/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */
/*  Subject to the MIT License. See LICENSE file in top-level directory. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using Upstream.System.Records;

namespace Upstream.System.Records.DataSets
{
    /// <summary>
    /// Implements IRecordFieldType using an ADO.Net DataColumn
    /// </summary>
    public class DataColumnFieldType
    : IRecordFieldType<object>
    {
        private DataColumn _dataColumn;

        /// <summary>
        /// Create A DataColumnFieldType that is not attached to a data column
        /// </summary>
        public DataColumnFieldType()
        {
        }

        /// <summary>
        /// Create a DataColumnFieldType that will be attached to a specific data column
        /// </summary>
        /// <param name="dataColumn"></param>
        public DataColumnFieldType(DataColumn dataColumn)
        {
            AttachTo(dataColumn);
        }

        /// <summary>
        /// Attach to a different data column object
        /// </summary>
        /// <param name="dataColumn"></param>
        public void AttachTo(DataColumn dataColumn)
        {
            _dataColumn = dataColumn;
        }

        /// <summary>
        /// Get the base data column, if there is one
        /// </summary>
        public DataColumn DataColumn
        {
            get
            {
                return _dataColumn;
            }
        }

        /// <summary>
        /// Get the base data column, raise an exception if there is no base column
        /// </summary>
        /// <returns></returns>
        private DataColumn GetDataColumn()
        {
            if (null == DataColumn)
            {
                throw new InvalidOperationException("DataColumnFieldType is not attached to a DataColumn object");
            }

            return DataColumn;
        }

        /// <summary>
        /// Get the data type associated with the data column
        /// </summary>
        public Type SystemType
        {
            get
            {
                return GetDataColumn().DataType;
            }
        }

        /// <summary>
        /// Compare two field values for this field type
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            IComparer comparer = Comparer.Default;

            return comparer.Compare(x, y);
        }

        /// <summary>
        /// Compare two field values for this field type for equivalence
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public new bool Equals(object x, object y)
        {
            return (0 == Compare(x, y));


        }

        /// <summary>
        /// Get an equivalence hash code for a field value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(object obj)
        {
            int hashCode = 0;

            if (null != obj)
            {
                hashCode = obj.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Do some basic checks to determine if a field value is compatible with this data column
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool IsValid(object fieldValue)
        {
            bool fieldValueIsValid = false;
            DataColumn column = GetDataColumn();

            if (null == fieldValue)
            {
                if (column.AllowDBNull)
                {
                    fieldValueIsValid = true;
                }
            }
            else if (SystemType.IsAssignableFrom(fieldValue.GetType()))
            {
                fieldValueIsValid = true;
            }

            return fieldValueIsValid;
        }

    } // /class

} // /namespace
