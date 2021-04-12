using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers.Records
{
    public static class RecordsFactory
    {
        public static TRecord? Read<TRecord>(Stream stream, byte opCode) where TRecord : Record
        {
            var header = ReadHeader(stream);
            int dataLen = stream.ReadInt32();
            if (header["op"][0] != opCode)
            {
                stream.Position += dataLen;
                return null;
            }

            var res = CreateRecord(header) as TRecord;
            res!.Data = new byte[dataLen];
            stream.Read(res!.Data, 0, dataLen);
            return res;
        }
        
        public static Record? Read(Stream stream, byte[] opCodes) 
        {
            var header = ReadHeader(stream);
            int dataLen = stream.ReadInt32();
            if (!opCodes.Contains(header["op"][0]))
            {
                stream.Position += dataLen;
                return null;
            }

            var res = CreateRecord(header);
            res!.Data = new byte[dataLen];
            stream.Read(res!.Data, 0, dataLen);
            return res;
        }
        
        public static Record ReadHeaders(Stream stream)
        {
            var header = ReadHeader(stream);
            int dataLen = stream.ReadInt32();
            stream.Position += dataLen;
            return CreateRecord(header);
        }

        public static (string name, byte[] data) ReadField(Stream stream)
        {
            int len = stream.ReadInt32();
            var buffer = stream.ReadBytes(len).Split((byte) '=').ToArray();
            if (buffer.Length > 2)
            {
                buffer[1] = buffer.Skip(1).ToArray().Join((byte) '=');
            }

            return (Encoding.UTF8.GetString(buffer[0]), buffer[1]);
        }

        #region Private

        private static Dictionary<string, byte[]> ReadHeader(Stream stream)
        {
            long startPos = stream.Position;
            int headerLen = stream.ReadInt32();
            var header = new Dictionary<string, byte[]>();
            while (stream.Position <= startPos + headerLen)
            {
                var (name, data) = ReadField(stream);
                header[name] = data;
            }

            return header;
        }

        private static Record CreateRecord(Dictionary<string, byte[]> header) =>
                header["op"][0] switch
                {
                    0x02 => new MessageData(header),
                    0x03 => new BagHeader(header),
                    0x04 => new IndexData(header),
                    0x05 => new Chunk(header),
                    0x06 => new ChunkInfo(header),
                    0x07 => new Connection(header),
                    _ => throw new ArgumentOutOfRangeException($"Unknown record op code: {header["op"][0]}"),
                };

        #endregion
    }
}