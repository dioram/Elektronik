using System;
using System.Collections.Generic;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class Record
    {
        public readonly byte[] Data;
        public readonly byte Op;

        public Record((Dictionary<string, byte[]> header, byte[] data) record)
        {
            Header = record.header;
            Op = Header["op"][0];
            Data = record.data;
        }

        #region Protected definitions

        protected readonly Dictionary<string, byte[]> Header;

        #endregion
    }
}