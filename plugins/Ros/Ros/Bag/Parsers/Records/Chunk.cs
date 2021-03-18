using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Elektronik.RosPlugin.Common;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    [Serializable]
    public class Chunk : Record
    {
        public const byte OpCode = 0x05;

        public enum CompressionType
        {
            None,
            Bz2,
        }

        public readonly CompressionType Compression;
        public readonly int Size;
        
        public Chunk((Dictionary<string, byte[]> header, byte[] data) record) : base(record)
        {
            if (Op != OpCode) throw new ParsingException("Can't read chunk");

            var str = Encoding.UTF8.GetString(Header["compression"]);
            switch (str)
            {
                case "none":
                    Compression = CompressionType.None;
                    break;
                case "bz2":
                    Compression = CompressionType.Bz2;
                    break;
                default:
                    throw new NotSupportedException($"{str} compression is not supported");
            }

            Size = BitConverter.ToInt32(Header["size"], 0);
        }

        public IEnumerable<MessageData> Unchunk()
        {
            if (Compression == CompressionType.Bz2)
                throw new NotSupportedException("Bz2 compression is not supported.");
            var stream = new MemoryStream(Data);

            while (stream.Position < stream.Length)
            {
                var res = RecordsFactory.Read(stream);
                switch (res)
                {
                    case Connection connection:
                        _connections.Add(connection);
                        break;
                    case MessageData message:
                        message.SetTopic(_connections);
                        yield return message;
                        break;
                }
            }
        }

        #region Private

        private readonly List<Connection> _connections = new List<Connection>();

        #endregion
    }
}