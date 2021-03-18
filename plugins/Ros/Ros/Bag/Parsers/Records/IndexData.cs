using System;
using System.Collections.Generic;
using Elektronik.RosPlugin.Common;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class IndexData: Record
    {
        public const byte OpCode = 0x04;

        public readonly int Version;
        public readonly int ConnectionId;
        public readonly int Count;
        
        public IndexData((Dictionary<string, byte[]> header, byte[] data) record) : base(record)
        {
            if (Op != OpCode) throw new ParsingException("Can't read IndexData");

            Version = BitConverter.ToInt32(Header["ver"], 0);
            ConnectionId = BitConverter.ToInt32(Header["conn"], 0);
            Count = BitConverter.ToInt32(Header["count"], 0);
        }
    }
}