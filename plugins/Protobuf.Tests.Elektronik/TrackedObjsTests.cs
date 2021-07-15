using System;
using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class TrackedObjsTests : TestsBase
    {
        private readonly TrackedObjPb[] _objects;
        private static readonly string Filename = $"{nameof(TrackedObjsTests)}.dat";
        private static int _timestamp = 0;
        private static readonly Random Rand = new();

        public TrackedObjsTests()
        {
            _objects = Enumerable.Range(0, 3).Select(id => new TrackedObjPb()
            {
                Id = id,
                Orientation = new Vector4Pb() {W = 1,},
                Position = new Vector3Pb(),
            }).ToArray();
        }

        [Test, Order(1), Explicit]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            _objects[0].Position = new Vector3Pb();
            _objects[1].Position = new Vector3Pb {X = 0.5,};
            _objects[2].Position = new Vector3Pb {X = -0.5,};
            _objects[0].Color = new ColorPb {R = 255,};
            _objects[1].Color = new ColorPb {G = 255,};
            _objects[2].Color = new ColorPb {B = 255,};
            packet.TrackedObjs.Data.Add(_objects);

            using var file = File.Open(Filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2), Explicit]
        public void Update()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            _objects[0].Position = new Vector3Pb {X = 0.0, Z = 0.5};
            _objects[1].Position = new Vector3Pb {X = 0.5, Z = 0.5};
            _objects[2].Position = new Vector3Pb {X = -0.5, Z = 0.5};
            packet.TrackedObjs.Data.Clear();
            packet.TrackedObjs.Data.Add(_objects);
            var response = MapClient.Handle(packet);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3), Repeat(5), Explicit]
        public void UpdateRandom()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
            };
            _objects[0].Position = new Vector3Pb
                    {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()};
            _objects[1].Position = new Vector3Pb
                    {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()};
            _objects[2].Position = new Vector3Pb
                    {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()};
            packet.TrackedObjs.Data.Clear();
            packet.TrackedObjs.Data.Add(_objects);
            var response = MapClient.Handle(packet);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4), Explicit]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            packet.TrackedObjs.Data.Add(new[] {_objects[1]});
            var response = MapClient.Handle(packet);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5), Explicit]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            var response = MapClient.Handle(packet);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}