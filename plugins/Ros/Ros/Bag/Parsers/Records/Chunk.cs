using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Elektronik.RosPlugin.Common;
using ICSharpCode.SharpZipLib.BZip2;

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
        
        public Chunk(Dictionary<string, byte[]> header) : base(header)
        {
            if (Op != OpCode) throw new ParsingException("Can't read chunk");

            var str = Encoding.UTF8.GetString(Header["compression"]);
            Compression = str switch
            {
                "none" => CompressionType.None,
                "bz2" => CompressionType.Bz2,
                _ => throw new NotSupportedException($"{str} compression is not supported")
            };

            Size = BitConverter.ToInt32(Header["size"], 0);
        }

        public IEnumerable<MessageData> Unchunk(int[]? allowedTopicIds = null)
        {
            using var inStream = new MemoryStream(Data!);
            var stream = new MemoryStream();
            if (Compression == CompressionType.Bz2)
            {
                BZip2.Decompress(inStream, stream, false);
            }
            else
            {
                stream = inStream;
            }

            stream.Position = 0;
            var res = new List<MessageData>(50);
            while (stream.Position < stream.Length)
            {
                var message = RecordsFactory.Read<MessageData>(stream, MessageData.OpCode);
                if (message != null 
                    && (allowedTopicIds == null || allowedTopicIds.Contains(message.ConnectionId)))
                {
                    res.Add(message);
                }
            }

            stream.Dispose();
            return res;
        }
    }
}