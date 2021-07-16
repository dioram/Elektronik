using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class InfinitePlanesTests : TestsBase
    {
        private readonly InfinitePlanePb[] _planes;
        private static readonly string Filename = $"{nameof(InfinitePlanesTests)}.dat";

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

        public InfinitePlanesTests()
        {
            _planes = Enumerable.Range(0, 5).Select(id => new InfinitePlanePb
            {
                Id = id,
                Message = $"{id}",
                Color = _colors[id],
                Normal = _normals[id],
                Offset = _offsets[id],
            }).ToArray();
        }

        [Test, Explicit]
        public void Grid()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb
            {
                Id = -10000,
                Message = "Grid",
                Normal = new Vector3Pb {X = 1, Y = 1, Z = 1},
                Offset = new Vector3Pb {X = 1, Y = 1, Z = 1},
            });

            using var file = File.Open(Filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(1), Explicit]
        public void Create()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(_planes);

            using var file = File.Open(Filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2), Explicit]
        public void UpdateOffsets()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb {Id = 0, Offset = new Vector3Pb {X = 0, Y = 0, Z = 1}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb {Id = 1, Offset = new Vector3Pb {X = 0, Y = 1, Z = 0}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb {Id = 2, Offset = new Vector3Pb {X = 1, Y = 0, Z = 0}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb {Id = 3, Offset = new Vector3Pb {X = 0, Y = 1, Z = 1}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb {Id = 4, Offset = new Vector3Pb {X = 1, Y = 1, Z = 0}});

            packet.InfinitePlanes.Data.Add(_planes);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3), Explicit]
        public void UpdateNormals()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb
                                                   {Id = 0, Normal = new Vector3Pb {X = -10, Y = -10, Z = -10}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb
                                                   {Id = 1, Normal = new Vector3Pb {X = -10, Y = -10, Z = -10}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb
                                                   {Id = 2, Normal = new Vector3Pb {X = -10, Y = -10, Z = -50}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb
                                                   {Id = 3, Normal = new Vector3Pb {X = -50, Y = -10, Z = -10}});
            packet.InfinitePlanes.Data.Add(new InfinitePlanePb
                                                   {Id = 4, Normal = new Vector3Pb {X = -10, Y = -50, Z = -10}});

            packet.InfinitePlanes.Data.Add(_planes);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4), Explicit]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(
                new InfinitePlanePb {Id = 0, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.InfinitePlanes.Data.Add(
                new InfinitePlanePb {Id = 1, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.InfinitePlanes.Data.Add(
                new InfinitePlanePb {Id = 2, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.InfinitePlanes.Data.Add(
                new InfinitePlanePb {Id = 3, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.InfinitePlanes.Data.Add(
                new InfinitePlanePb {Id = 4, Color = new ColorPb {R = 255, G = 255, B = 255}});

            packet.InfinitePlanes.Data.Add(_planes);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5), Explicit]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };

            packet.InfinitePlanes.Data.Add(new[] {_planes[1], _planes[3]});

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(6), Explicit]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}