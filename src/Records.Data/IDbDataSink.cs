/*  Copyright (c) 2017 Upstream Research, Inc.  All Rights Reserved.  */

using System;
using System.Collections.Generic;
using System.Data;

namespace Upstream.System.Records.Data
{
    /// <summary>
    /// Simple interface for objects that can pull records from an IDataReader
    /// </summary>
    public interface IDbDataSink
    {
        /// <summary>
        /// Read records from a data reader,
        /// what happens to the records is implementation-defined
        /// (though they will probably get stored somewhere)
        /// </summary>
        /// <param name="reader"></param>
        /// <remarks>
        /// This is modeled after DataTable.Load()
        /// </remarks>
        void Load(IDataReader reader);

    } // /interface

} // /namespace
