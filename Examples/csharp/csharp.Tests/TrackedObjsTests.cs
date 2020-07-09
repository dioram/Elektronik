using Elektronik.Common.Data.Pb;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csharp.Tests
{
    public class TrackedObjsTests : TestsBase
    {
        private TrackedObjPb[] m_objects;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            m_objects = Enumerable.Range(0, 3).Select(id => new TrackedObjPb()
            {
                Id = id,
                Rotation = new Vector4Pb() { W = 1, },
                Translation = new Vector3Pb(),
            }).ToArray();
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            m_objects[0].TrackColor = new ColorPb() { R = 255, };
            m_objects[1].TrackColor = new ColorPb() { G = 255, };
            m_objects[2].TrackColor = new ColorPb() { B = 255, };
            packet.TrackedObjs.Data.Add(m_objects);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packets = new List<PacketPb>();
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            for (int i = 0; i < 5; ++i)
            {
                packet.TrackedObjs.Data.Clear();
                m_objects[0].Translation.Z += 0.5;
                packet.TrackedObjs.Data.Add(m_objects[0]);
                packets.Add(new PacketPb(packet));
            }
            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            packet.TrackedObjs.Data.Add(new[] { m_objects[1] });
            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}
