using System;
using Elektronik.Common.Data.Pb;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Protobuf;

namespace csharp.Tests
{
    public class TrackedObjsTests : TestsBase
    {
        private TrackedObjPb[] m_objects;
        private string filename = $"{nameof(TrackedObjsTests)}.dat";

        public TrackedObjsTests()
        {
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
            m_objects[0].Translation = new Vector3Pb();
            m_objects[1].Translation = new Vector3Pb() { X = 0.5, };
            m_objects[2].Translation = new Vector3Pb() { X = -0.5, };
            m_objects[0].TrackColor = new ColorPb() { R = 255, };
            m_objects[1].TrackColor = new ColorPb() { G = 255, };
            m_objects[2].TrackColor = new ColorPb() { B = 255, };
            packet.TrackedObjs.Data.Add(m_objects);
            
            using var file = File.Open(filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = m_mapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            m_objects[0].Translation = new Vector3Pb() { X = 0.0, Z = 0.5 };
            m_objects[1].Translation = new Vector3Pb() { X = 0.5, Z = 0.5 };
            m_objects[2].Translation = new Vector3Pb() { X = -0.5, Z = 0.5 };
            packet.TrackedObjs.Data.Clear();
            packet.TrackedObjs.Data.Add(m_objects);
            var response = m_mapClient.Handle(packet);

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3), Repeat(5)]
        public void UpdateRandom()
        {
            var rand = new Random();
            var packet = new PacketPb()
            {
                    Action = PacketPb.Types.ActionType.Update,
                    TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            m_objects[0].Translation = new Vector3Pb() { X = rand.NextDouble(), Y = rand.NextDouble(),  Z = rand.NextDouble() };
            m_objects[1].Translation = new Vector3Pb() { X = rand.NextDouble(), Y = rand.NextDouble(),  Z = rand.NextDouble() };
            m_objects[2].Translation = new Vector3Pb() { X = rand.NextDouble(), Y = rand.NextDouble(),  Z = rand.NextDouble() };
            packet.TrackedObjs.Data.Clear();
            packet.TrackedObjs.Data.Add(m_objects);
            var response = m_mapClient.Handle(packet);

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            packet.TrackedObjs.Data.Add(new[] { m_objects[1] });
            var response = m_mapClient.Handle(packet);

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var response = m_mapClient.Handle(packet);

            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}
