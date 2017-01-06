using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upstream.System.Records
{
    /// <summary>
    /// Defines a collector-type interface that provides a working-record
    /// that can be written-back to a record collection
    /// </summary>
    public interface IRecordCollectionBuilder<TValue>
    : IRecordCollector<TValue>
    {
        /// <summary>
        /// Get the current working item which can be modified before the record is added
        /// </summary>
        IRecordAccessor<TValue> Current { get; }

        /// <summary>
        /// Initialize the state of the current object
        /// (set fields to default values)
        /// </summary>
        void InitializeCurrentItem();

        /// <summary>
        /// Try to add the current item to the underlying collection
        /// </summary>
        /// <returns>
        /// true if the item was successfully added, otherwise false
        /// </returns>
        bool AddCurrentItem();

    } // /interface
} // /class
