using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class PlanesTests : TestsBase
    {
        private readonly PlanePb[] _planes;
        private static readonly string Filename = $"{nameof(PlanesTests)}.dat";

        private readonly ColorPb[] _colors =
        {
            new() {B = 255, G = 0, R = 0},
            new() {B = 0, G = 0, R = 255},
            new() {B = 0, G = 255, R = 0},
            new() {B = 255, G = 0, R = 255},
            new() {B = 255, G = 255, R = 0},
        };

        private readonly Vector3Pb[] _offsets =
        {
            new() {X = 0, Y = 0, Z = 0},
            new() {X = 0, Y = 0, Z = 0},
            new() {X = 0, Y = 0, Z = 50},
            new() {X = 50, Y = 0, Z = 0},
            new() {X = 0, Y = 50, Z = 0},
        };

        private readonly Vector3Pb[] _normals =
        {
            new() {X = 0, Y = 1, Z = 0},
            new() {X = 1, Y = 0, Z = 0},
            new() {X = 0, Y = 0, Z = 1},
            new() {X = 1, Y = 0, Z = 1},
            new() {X = 0, Y = 1, Z = 0},
        };

        public PlanesTests()
        {
            _planes = Enumerable.Range(0, 5).Select(id => new PlanePb
            {
                Id = id,
                Message = $"{id}",
                Color = _colors[id],
                Normal = _normals[id],
                Offset = _offsets[id],
            }).ToArray();
        }

        [Test, Order(1), Explicit]
        public void Create()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(_planes);

            SendAndCheck(packet, Filename, true);
        }

        [Test, Order(2), Explicit]
        public void UpdateOffsets()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(new PlanePb {Id = 0, Offset = new Vector3Pb {X = 0, Y = 0, Z = 100}});
            packet.Planes.Data.Add(new PlanePb {Id = 1, Offset = new Vector3Pb {X = 0, Y = 100, Z = 0}});
            packet.Planes.Data.Add(new PlanePb {Id = 2, Offset = new Vector3Pb {X = 100, Y = 0, Z = 0}});
            packet.Planes.Data.Add(new PlanePb {Id = 3, Offset = new Vector3Pb {X = 0, Y = 100, Z = 100}});
            packet.Planes.Data.Add(new PlanePb {Id = 4, Offset = new Vector3Pb {X = 100, Y = 100, Z = 0}});
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(3), Explicit]
        public void UpdateNormals()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(new PlanePb
                                                   {Id = 0, Normal = new Vector3Pb {X = -10, Y = -10, Z = -10}});
            packet.Planes.Data.Add(new PlanePb
                                                   {Id = 1, Normal = new Vector3Pb {X = -10, Y = -10, Z = -10}});
            packet.Planes.Data.Add(new PlanePb
                                                   {Id = 2, Normal = new Vector3Pb {X = -10, Y = -10, Z = -50}});
            packet.Planes.Data.Add(new PlanePb
                                                   {Id = 3, Normal = new Vector3Pb {X = -50, Y = -10, Z = -10}});
            packet.Planes.Data.Add(new PlanePb
                                                   {Id = 4, Normal = new Vector3Pb {X = -10, Y = -50, Z = -10}});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(4), Explicit]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(
                new PlanePb {Id = 0, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Planes.Data.Add(
                new PlanePb {Id = 1, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Planes.Data.Add(
                new PlanePb {Id = 2, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Planes.Data.Add(
                new PlanePb {Id = 3, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Planes.Data.Add(
                new PlanePb {Id = 4, Color = new ColorPb {R = 255, G = 255, B = 255}});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(5), Explicit]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                Planes = new PacketPb.Types.Planes(),
            };

            packet.Planes.Data.Add(new[] {_planes[1], _planes[3]});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(6), Explicit]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Planes = new PacketPb.Types.Planes(),
            };
            
            SendAndCheck(packet, Filename);
        }
    }
}