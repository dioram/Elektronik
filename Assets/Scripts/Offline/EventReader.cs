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
        public static GState[] AnalyzeFile(string path)
        {
            GState[] result = null;
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
                result = new GState[eventsCount];
                GState lastFrame = new GState();
                int[] offsetTable = Enumerable.Range(0, eventsCount).Select(_ => br.ReadInt32()).ToArray();
                for (int i = 0; i < eventsCount; ++i)
                {
                    SlamEventType slamEventType = (SlamEventType)br.ReadByte();
                    switch (slamEventType)
                    {
                        case SlamEventType.MainThreadEvent:
                            {
                                lastFrame = lastFrame.CloneUpdate(MainThreadEvent.Parse(br));
                                break;
                            }
                        default:
                            break;
                    }
                    result[i] = lastFrame;
                }
            }
            return result;
        }
    }
}