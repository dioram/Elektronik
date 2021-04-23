using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests
{
    public class InfinitePlanesTests : TestsBase
    {
        private readonly InfinitePlanePb[] _planes;
        private string filename = nameof(InfinitePlanesTests);

        private readonly ColorPb[] _colors =
        {
            new ColorPb {B = 255, G = 0, R = 0},
            new ColorPb {B = 0, G = 0, R = 255},
            new ColorPb {B = 0, G = 255, R = 0},
            new ColorPb {B = 255, G = 0, R = 255},
            new ColorPb {B = 255, G = 255, R = 0},
        };

        private readonly Vector3Pb[] _offsets =
        {
            new Vector3Pb {X = 0, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 0, Z = 50},
            new Vector3Pb {X = 50, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 50, Z = 0},
        };

        private readonly Vector3Pb[] _normals =
        {
            new Vector3Pb {X = 0, Y = 1, Z = 0},
            new Vector3Pb {X = 1, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 0, Z = 1},
            new Vector3Pb {X = 1, Y = 0, Z = 1},
            new Vector3Pb {X = 0, Y = 1, Z = 0},
        };

        public InfinitePlanesTests()
        {
            _planes = Enumerable.Range(0, 5).Select(id => new InfinitePlanePb()
            {
                Id = id,
                Message = $"{id}",
                Color = _colors[id],
                Normal = _normals[id],
                Offset = _offsets[id],
            }).ToArray();
        }
        
        [Test, Order(0)]
        public void Grid()
        {
            var packet = new PacketPb()
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
            
            using var file = File.Open(filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
        
        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(_planes);
            
            using var file = File.Open(filename, FileMode.Create);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            _planes[0].Color = new ColorPb{B = 128, G = 128, R = 128};
            _planes[2].Offset = new Vector3Pb {X = 0, Y = 0, Z = -50};
            _planes[4].Normal = new Vector3Pb {X = 1, Y = 1, Z = 1};

            packet.InfinitePlanes.Data.Add(_planes);
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };

            packet.InfinitePlanes.Data.Add(new[] { _planes[1], _planes[3] });
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            
            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}