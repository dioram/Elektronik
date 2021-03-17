using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Elektronik.Ros.Rosbag.Parsers.Records
{
    public static class RecordsFactory
    {
        static RecordsFactory()
        {
            Records = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => typeof(Record).IsAssignableFrom(t))
                    .Where(t => t != typeof(Record))
                    .ToDictionary(t => (byte) t.GetField("OpCode").GetValue(null));
        }

        public static Record? Read(Stream stream, bool onlyConnections = false)
        {
            var record = ReadRecord(stream, onlyConnections);
            if (record == null) return null;
            return (Record) Activator.CreateInstance(Records[record.Value.header["op"][0]], record);
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

        private static readonly Dictionary<byte, Type> Records;

        private static (Dictionary<string, byte[]> header, byte[] data)? ReadRecord(Stream stream, bool onlyConnection)
        {
            var header = ReadHeader(stream);
            int dataLen = stream.ReadInt32();
            if (onlyConnection && header["op"][0] != Connection.OpCode)
            {
                stream.Position += dataLen;
                return null;
            }
            var data = stream.ReadBytes(dataLen);
            return (header, data);
        }

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

        #endregion
    }
}