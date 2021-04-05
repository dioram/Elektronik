using System;
using System.Collections.Generic;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class Record
    {
        public byte[]? Data;
        public readonly byte Op;

        public Record(Dictionary<string, byte[]> header)
        {
            Header = header;
            Op = Header["op"][0];
        }

        #region Protected definitions

        protected readonly Dictionary<string, byte[]> Header;

        #endregion
    }
}