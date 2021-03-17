using System;
using System.Collections.Generic;

namespace Elektronik.Ros.Rosbag.Parsers.Records
{
    [Serializable]
    public class MessageData : Record
    {
        public const byte OpCode = 0x02;

        public readonly int ConnectionId;
        
        public readonly long Timestamp;
        
        public string? TopicType { get; private set; }
        
        public string? TopicName { get; private set; }
        
        public MessageData((Dictionary<string, byte[]> header, byte[] data) record) : base(record)
        {
            if (Op != OpCode) throw new ParsingException("Can't read MessageData");

            ConnectionId = BitConverter.ToInt32(Header["conn"], 0);
            Timestamp = BitConverter.ToInt64(Header["time"], 0);
        }

        public void SetTopic(IEnumerable<Connection> connections)
        {
            foreach (var connection in connections)
            {
                if (connection.Id != ConnectionId) continue;
                TopicType = connection.Type;
                TopicName = connection.Topic;
                break;
            }
        }
    }
}