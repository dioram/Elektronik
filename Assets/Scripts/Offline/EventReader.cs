using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Common;
using Elektronik.Common.Events;

namespace Elektronik.Offline
{
    public static class EventReader
    {
        public static ISlamEvent[] AnalyzeFile(string path, ISlamEventDataConverter converter)
        {
            ISlamEvent[] result = null;
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
                result = new ISlamEvent[eventsCount];
                int[] offsetTable = Enumerable.Range(0, eventsCount).Select(_ => br.ReadInt32()).ToArray();
                for (int i = 0; i < eventsCount; ++i)
                {
                    br.BaseStream.Seek(offsetTable[i], SeekOrigin.Begin);
                    UnityEngine.Debug.Log(String.Format("{0}. offset: {1} (0x{1:X8})", i, offsetTable[i]));
                    ISlamEvent slamEvent = ParseEvent(br);
                    converter.Convert(ref slamEvent);
                    result[i] = slamEvent;
                }
            }
            return result;
        }

        private static ISlamEvent ParseEvent(BinaryReader stream)
        {
            SlamEventType type = (SlamEventType)stream.ReadByte();
            switch (type)
            {
                case SlamEventType.MainThreadEvent:
                    {
                        return MainThreadEvent.Parse(stream);
                    }
                case SlamEventType.GlobalMap:
                    {
                        return GlobalMapEvent.Parse(stream);
                    }
                case SlamEventType.LMPointsRemoval:
                    {
                        return RemovalEvent.Parse(stream, type);
                    }
                case SlamEventType.LMPointsFusion:
                    {
                        return PointsFusionEvent.Parse(stream, type);
                    }
                case SlamEventType.LMObservationRemoval:
                    {
                        return RemovalEvent.Parse(stream, type);
                    }
                case SlamEventType.LMLBA:
                    {
                        return MapModificationEvent.Parse(stream, type);
                    }
                case SlamEventType.LCPointsFusion:
                    {
                        return PointsFusionEvent.Parse(stream, type);
                    }
                case SlamEventType.LCOptimizeEssentialGraph:
                    {
                        return MapModificationEvent.Parse(stream, type);
                    }
                case SlamEventType.LCGBA:
                    {
                        return MapModificationEvent.Parse(stream, type);
                    }
                case SlamEventType.LCLoopClosingTry:
                    {
                        return LoopClosingTryEvent.Parse(stream);
                    }
                default:
                    return null;
            }
        }
    }
}