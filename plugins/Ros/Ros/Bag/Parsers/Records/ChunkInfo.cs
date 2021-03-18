using System;
using System.Collections.Generic;
using Elektronik.RosPlugin.Common;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class ChunkInfo : Record
    {
        public const byte OpCode = 0x06;

        public readonly int Version;
        public readonly long ChunkPos;
        public readonly long StartTime;
        public readonly long EndTime;
        public readonly int Count;
        
        public ChunkInfo((Dictionary<string, byte[]> header, byte[] data) record) : base(record)
        {
            if (Op != OpCode) throw new ParsingException("Can't read ChunkInfo");

            Version = BitConverter.ToInt32(Header["ver"], 0);
            ChunkPos = BitConverter.ToInt64(Header["chunk_pos"], 0);
            StartTime = BitConverter.ToInt64(Header["start_time"], 0);
            EndTime = BitConverter.ToInt64(Header["end_time"], 0);
            Count = BitConverter.ToInt32(Header["count"], 0);
        }
    }
}