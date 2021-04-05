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
        private string _type = "";

        public string Type => string.IsNullOrEmpty(_type) ? GetTopicType() : _type;

        public Connection(Dictionary<string, byte[]> header) : base(header)
        {
            if (Op != OpCode) throw new FileLoadException("Can't read connection");

            Id = BitConverter.ToInt32(Header["conn"], 0);
            Topic = Encoding.UTF8.GetString(Header["topic"]);
        }

        public override string ToString()
        {
            return $"{Type}    {Topic}";
        }

        private string GetTopicType()
        {
            var stream = new MemoryStream(Data!);
            while (stream.Position < stream.Length)
            {
                var (name, data) = RecordsFactory.ReadField(stream);
                if (name == "type")
                {
                    _type = Encoding.UTF8.GetString(data);
                    return _type;
                }
            }

            throw new ParsingException($"Type not found for topic {Topic}");
        }
    }
}