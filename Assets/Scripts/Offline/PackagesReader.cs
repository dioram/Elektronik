using Elektronik.Common;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public class PackagesReader
    {
        public static Package[] AnalyzeFile(string path, IPackageCSConverter converter)
        {
            Package[] result = null;
            using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
            {
                if (br.ReadUInt32() != 0xDEADBEEF)
                    throw new FileLoadException("broken file (1st magic number)");
                br.BaseStream.Seek(br.BaseStream.Length - sizeof(int), SeekOrigin.Begin);
                int tableOffset = br.ReadInt32();
                br.BaseStream.Seek(tableOffset, SeekOrigin.Begin);
                if (br.ReadUInt32() != 0xDEADBEEF)
                    throw new FileLoadException("broken file (2nd magic number)");
                int eventsCount = br.ReadInt32();
                result = new Package[eventsCount];
                int[] offsetTable = Enumerable.Range(0, eventsCount).Select(_ => br.ReadInt32()).ToArray();
                for (int i = 0; i < eventsCount; ++i)
                {
                    br.BaseStream.Seek(offsetTable[i], SeekOrigin.Begin);
                    Debug.Log(String.Format("{0}. offset: {1} (0x{1:X8})", i, offsetTable[i]));
                    long packageLength = 
                        i == eventsCount - 1 ?
                        br.BaseStream.Length - sizeof(int) /*table offset*/ - offsetTable[i] 
                        : offsetTable[i + 1] - offsetTable[i];
                    byte[] rawPackage = br.ReadBytes((int)packageLength);
                    Package package = Package.Parse(rawPackage);
                    converter.Convert(ref package);
                    result[i] = package;
                }
            }
            return result;
        }
    }
}
