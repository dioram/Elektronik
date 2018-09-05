using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Offline.Events;

namespace Elektronik.Offline
{
    public static class EventReader
    {
        public static ISlamEvent[] AnalyzeFile(string path)
        {
            ISlamEvent[] result = null;
            using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
            {
                if (br.ReadUInt32() != 0xDEADBEEF)
                    throw new FileLoadException("broken file (1st magic number)");
                br.BaseStream.Seek(sizeof(int), SeekOrigin.End);
                int tableOffset = br.ReadInt32();
                br.BaseStream.Seek(tableOffset, SeekOrigin.Begin);
                if (br.ReadUInt32() != 0xDEADBEEF)
                    throw new FileLoadException("broken file (2nd magic number)");
                int eventsCount = br.ReadInt32();
                result = new ISlamEvent[eventsCount];
                int[] offsetTable = Enumerable.Range(0, eventsCount).Select(_ => br.ReadInt32()).ToArray();
                for (int i = 0; i < eventsCount; ++i)
                {
                    SlamEventType slamEventType = (SlamEventType)br.ReadByte();
                    result[i] = ParseEventByType(slamEventType, br);
                }
            }
            return result;
        }

        private static ISlamEvent ParseEventByType(SlamEventType type, BinaryReader stream)
        {
            switch (type)
            {
                case SlamEventType.MainThreadEvent:
                    {
                        return MainThreadEvent.Parse(stream);
                    }
                case SlamEventType.LMPointsRemoval:
                    {
                        return RemovalEvent.Parse(stream, SlamEventType.LMPointsRemoval);
                    }
                case SlamEventType.LMPointsFusion:
                    {
                        return PointsFusionEvent.Parse(stream, SlamEventType.LMPointsFusion);
                    }
                case SlamEventType.LMObservationRemoval:
                    {
                        return RemovalEvent.Parse(stream, SlamEventType.LMObservationRemoval);
                    }
                case SlamEventType.LMLBA:
                    {
                        return MapModificationEvent.Parse(stream, SlamEventType.LMLBA);
                    }
                case SlamEventType.LCPointsFusion:
                    {
                        return PointsFusionEvent.Parse(stream, SlamEventType.LCPointsFusion);
                    }
                case SlamEventType.LCOptimizeEssentialGraph:
                    {
                        return MapModificationEvent.Parse(stream, SlamEventType.LCOptimizeEssentialGraph);
                    }
                case SlamEventType.LCGBA:
                    {
                        return MapModificationEvent.Parse(stream, SlamEventType.LCGBA);
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