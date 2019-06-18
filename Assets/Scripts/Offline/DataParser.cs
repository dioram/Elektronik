﻿using Elektronik.Common;
using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Elektronik.Common.Extensions;

namespace Elektronik.Offline
{
    public class DataSource
    {
        private DataParser m_parser;
        public DataSource()
        {
            var converter = new Camera2Unity3dPackageConverter(Matrix4x4.Scale(Vector3.one * FileModeSettings.Current.Scaling));
            m_parser = 
                new IChainable<DataParser>[] 
                {
                    new SlamPackageParser(converter),
                    new TrackingPackageParser(converter),
                }.BuildChain();
        }

        public event Action<IList<IPackage>> DataReady;
        public async void ParseData(string path)
        {
            Debug.Log("ANALYSIS STARTED");
            var task = new Task<IList<IPackage>>(() => Parse(path));
            DataReady?.Invoke(await task);
            Debug.Log("ANALYSIS FINISHED");
        }

        private IList<IPackage> Parse(string path)
        {
            IList<IPackage> result = null;
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
                result = new List<IPackage>(eventsCount);
                int[] offsetTable = Enumerable.Range(0, eventsCount).Select(_ => br.ReadInt32()).ToArray();
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                byte[] data = br.ReadBytes((int)br.BaseStream.Length);
                Parallel.For(0, eventsCount, (i) =>
                {
                    Debug.Log($"{i}. offset: {offsetTable[i]} (0x{offsetTable[i]:X8})");
                    long packageLength =
                        i == eventsCount - 1 ?
                        br.BaseStream.Length - sizeof(int) /*table offset*/ - offsetTable[i]
                        : offsetTable[i + 1] - offsetTable[i];
                    byte[] rawPackage = new byte[(int)packageLength];
                    Buffer.BlockCopy(data, offsetTable[i], rawPackage, 0, (int)packageLength);
                    m_parser.Parse(rawPackage, 0, out IPackage package);
                    result[i] = package;
                });
            }
            return result;
        }
    }
}
