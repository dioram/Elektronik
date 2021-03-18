using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Elektronik.RosPlugin.Common;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class Connection : Record
    {
        public const byte OpCode = 0x07;

        public readonly int Id;
        public readonly string Topic;
        public readonly string Type;
        
        public Connection((Dictionary<string, byte[]> header, byte[] data) record) : base(record)
        {
            if (Op != OpCode) throw new FileLoadException("Can't read connection");

            Id = BitConverter.ToInt32(Header["conn"], 0);
            Topic = Encoding.UTF8.GetString(Header["topic"]);

            var stream = new MemoryStream(Data);
            while (stream.Position < stream.Length)
            {
                var (name, data) = RecordsFactory.ReadField(stream);
                if (name == "type")
                {
                    Type = Encoding.UTF8.GetString(data);
                    break;
                }
            }

            if (Type is null) throw new ParsingException($"Type not found for topic {Topic}");
        }

        public override string ToString()
        {
            return $"{Type}    {Topic}";
        }
    }
}