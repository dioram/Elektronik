using Elektronik.Common.Data.Pb;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csharp
{
    public class TrackedObjectsTest : ITest
    {
        // imitate slam maps
        private TrackedObjPb[] m_objects;

        public TrackedObjectsTest()
        {
            m_objects = Enumerable.Range(0, 3).Select(id => new TrackedObjPb()
            {
                Id = id,
                Rotation = new Vector4Pb() { W = 1, },
                Translation = new Vector3Pb(),
            }).ToArray();
        }

        public IEnumerable<PacketPb> Create()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjsPacket = new PacketPb.Types.TrackedObjsPacket(),
            };
            m_objects[0].TrackColor = new ColorPb() { R = 255, };
            m_objects[1].TrackColor = new ColorPb() { G = 255, };
            m_objects[2].TrackColor = new ColorPb() { B = 255, };
            packet.TrackedObjsPacket.TrackedObjs.Add(m_objects);

            return new[] { packet };
        }

        public IEnumerable<PacketPb> Update()
        {
            var packets = new List<PacketPb>();
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjsPacket = new PacketPb.Types.TrackedObjsPacket(),
            };
            for (int i = 0; i < 5; ++i)
            {
                packet.TrackedObjsPacket.TrackedObjs.Clear();
                m_objects[0].Translation.Z += 0.5;
                packet.TrackedObjsPacket.TrackedObjs.Add(m_objects[0]);
                packets.Add(new PacketPb(packet));
            }
            return packets;
        }

        public IEnumerable<PacketPb> Remove()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                TrackedObjsPacket = new PacketPb.Types.TrackedObjsPacket(),
            };
            packet.TrackedObjsPacket.TrackedObjs.Add(new[] { m_objects[1] });
            return new[] { packet };
        }

        public IEnumerable<PacketPb> Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                TrackedObjsPacket = new PacketPb.Types.TrackedObjsPacket(),
            };

            return new[] { packet };
        }
    }
}
