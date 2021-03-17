using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Elektronik.Ros.Rosbag.Parsers
{
    public static class BitConversionExt
    {
        public static int ReadInt32(this Stream data)
        {
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }
        
        public static byte[] ReadBytes(this Stream data, int count)
        {
            var buffer = new byte[count];
            data.Read(buffer, 0, count);
            return buffer;
        }

        public static IEnumerable<byte[]> Split(this byte[] data, byte separator)
        {
            var res = new List<byte[]>();
            var lastSep = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == separator)
                {
                    res.Add(data.AsSpan(lastSep, i - lastSep).ToArray());
                    lastSep = i + 1;
                }
            }
            res.Add(data.AsSpan(lastSep, data.Length - lastSep).ToArray());

            return res;
        }

        public static byte[] Join(this byte[][] data, byte separator)
        {
            var res = new byte[data.Select(d => d.Length).Sum() + data.Length - 1];

            int index = 0;
            foreach (var arr in data)
            {
                foreach (var b in arr)
                {
                    res[index] = b;
                    index++;
                }

                if (index < res.Length)
                {
                    res[index] = separator;
                    index++;
                }
            }

            return res;
        }
    }
}