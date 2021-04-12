using System;
using NUnit.Framework;
using System.IO;
using System.Linq;
using Elektronik.Common.Data.Pb;
using Google.Protobuf;

namespace Protobuf.Tests
{
    public class TrackedObjsTests : TestsBase
    {
        private readonly TrackedObjPb[] _objects;
        private readonly string _filename = $"{nameof(TrackedObjsTests)}.dat";
        private static int _timestamp = 0;
        private static readonly Random Rand = new Random();

        public TrackedObjsTests()
        {
            _objects = Enumerable.Range(0, 3).Select(id => new TrackedObjPb()
            {
                    Id = id,
                    Rotation = new Vector4Pb() {W = 1,},
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
                    Timestamp = ++_timestamp,
                    Special = true,
            };
            _objects[0].Translation = new Vector3Pb();
            _objects[1].Translation = new Vector3Pb() {X = 0.5,};
            _objects[2].Translation = new Vector3Pb() {X = -0.5,};
            _objects[0].TrackColor = new ColorPb() {R = 255,};
            _objects[1].TrackColor = new ColorPb() {G = 255,};
            _objects[2].TrackColor = new ColorPb() {B = 255,};
            packet.TrackedObjs.Data.Add(_objects);

            using var file = File.Open(_filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                    Action = PacketPb.Types.ActionType.Update,
                    TrackedObjs = new PacketPb.Types.TrackedObjs(),
                    Timestamp = ++_timestamp,
                    Special = true,
            };
            _objects[0].Translation = new Vector3Pb() {X = 0.0, Z = 0.5};
            _objects[1].Translation = new Vector3Pb() {X = 0.5, Z = 0.5};
            _objects[2].Translation = new Vector3Pb() {X = -0.5, Z = 0.5};
            packet.TrackedObjs.Data.Clear();
            packet.TrackedObjs.Data.Add(_objects);
            var response = MapClient.Handle(packet);

            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3), Repeat(5)]
        public void UpdateRandom()
        {
            var packet = new PacketPb()
            {
                    Action = PacketPb.Types.ActionType.Update,
                    TrackedObjs = new PacketPb.Types.TrackedObjs(),
                    Timestamp = ++_timestamp,
            };
            _objects[0].Translation = new Vector3Pb()
                    {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()};
            _objects[1].Translation = new Vector3Pb()
                    {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()};
            _objects[2].Translation = new Vector3Pb()
                    {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()};
            packet.TrackedObjs.Data.Clear();
            packet.TrackedObjs.Data.Add(_objects);
            var response = MapClient.Handle(packet);

            using var file = File.Open(_filename, FileMode.Append);
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
                    Timestamp = ++_timestamp,
                    Special = true,
            };
            packet.TrackedObjs.Data.Add(new[] {_objects[1]});
            var response = MapClient.Handle(packet);

            using var file = File.Open(_filename, FileMode.Append);
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
                    Timestamp = ++_timestamp,
                    Special = true,
            };
            var response = MapClient.Handle(packet);

            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}