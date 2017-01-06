using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upstream.System.Records
{
    /// <summary>
    /// Provides the obvious implementation for IRecordFieldProperties
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class BasicRecordFieldProperties<TValue>
    : IRecordFieldProperties<TValue>
    {
        private Type _dataType;
        private IComparer<TValue> _sortComparer;
        private IEqualityComparer<TValue> _equalityComparer;

        public BasicRecordFieldProperties(
            Type dataType
            ,IComparer<TValue> sortComparer
            ,IEqualityComparer<TValue> equalityComparer
            )
        {
            if (null == dataType)
            {
                throw new ArgumentNullException("dataType");
            }
            if (null == sortComparer)
            {
                throw new ArgumentNullException("sortComparer");
            }
            if (null == equalityComparer)
            {
                throw new ArgumentNullException("equalityComparer");
            }

            _dataType = dataType;
            _sortComparer = sortComparer;
            _equalityComparer = equalityComparer;
        }

        public BasicRecordFieldProperties(
            Type dataType
            )
        : this(
             dataType
            ,Comparer<TValue>.Default
            ,EqualityComparer<TValue>.Default
            )
        {
        }

        public BasicRecordFieldProperties()
        : this(typeof(TValue))
        {
        }

        public Type DataType
        {
            get
            {
                return _dataType;
            }
        }

        public IComparer<TValue> ValueSortComparer
        {
            get
            {
                return _sortComparer;
            }
        }

        public IEqualityComparer<TValue> ValueEqualityComparer
        {
            get
            {
                return _equalityComparer;
            }
        }

    } // /class

} // /namespace
