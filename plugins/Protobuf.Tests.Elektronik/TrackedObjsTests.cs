using System;
using System.Linq;
using Elektronik.Protobuf.Data;
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

            SendAndCheck(packet, Filename);
        }

        [Test, Order(2), Explicit]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 0, Position = new Vector3Pb {X = 0, Z = 0.5f}});
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 1, Position = new Vector3Pb {X = 0.5f, Z = 0.5f}});
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 2, Position = new Vector3Pb {X = -0.5f, Z = 0.5f}});
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(3), Explicit]
        public void UpdateOrientations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 0, Orientation = new Vector4Pb {X = 1, W = 1}});
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 1, Orientation = new Vector4Pb {X = 1, W = 1}});
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 2, Orientation = new Vector4Pb {X = 1, W = 1}});
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(4), Explicit]
        public void UpdatePositionsRandom()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            packet.TrackedObjs.Data.Add(new TrackedObjPb
            {
                Id = 0, Position = new Vector3Pb {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()}
            });
            packet.TrackedObjs.Data.Add(new TrackedObjPb
            {
                Id = 1, Position = new Vector3Pb {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()}
            });
            packet.TrackedObjs.Data.Add(new TrackedObjPb
            {
                Id = 2, Position = new Vector3Pb {X = Rand.NextDouble(), Y = Rand.NextDouble(), Z = Rand.NextDouble()}
            });
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(5), Explicit]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 0, Color = new ColorPb{R = 255, G = 255, B = 255}});
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 1, Color = new ColorPb{R = 255, G = 255, B = 255}});
            packet.TrackedObjs.Data.Add(new TrackedObjPb {Id = 2, Color = new ColorPb{R = 255, G = 255, B = 255}});
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(6), Explicit]
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
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(7), Explicit]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
                Timestamp = ++_timestamp,
                Special = true,
            };
            
            SendAndCheck(packet, Filename);
        }
    }
}