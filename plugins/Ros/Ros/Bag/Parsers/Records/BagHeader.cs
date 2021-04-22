using System;
using System.Collections.Generic;
using Elektronik.RosPlugin.Common;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class BagHeader : Record
    {
        public const byte OpCode = 0x03;
        
        public readonly long IndexPos;
        public readonly int ConnectionsCount;
        public readonly long ChunkCount;
        
        public BagHeader(Dictionary<string, byte[]> header) : base(header)
        {
            if (Op != OpCode) throw new ParsingException("Can't read bag header");
            IndexPos = BitConverter.ToInt64(Header["index_pos"], 0);
            ConnectionsCount = BitConverter.ToInt32(Header["conn_count"], 0);
            ChunkCount = BitConverter.ToInt32(Header["chunk_count"], 0);
        }
    }
}