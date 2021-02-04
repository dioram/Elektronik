using System.Linq;
using Elektronik.Common.Data.Pb;
using NUnit.Framework;

namespace csharp.Tests
{
    public class InfinitePlanesTests : TestsBase
    {
        private InfinitePlanePb[] m_planes;

        private readonly ColorPb[] m_colors =
        {
            new ColorPb {B = 255, G = 0, R = 0},
            new ColorPb {B = 0, G = 0, R = 255},
            new ColorPb {B = 0, G = 255, R = 0},
            new ColorPb {B = 255, G = 0, R = 255},
            new ColorPb {B = 255, G = 255, R = 0},
        };

        private readonly Vector3Pb[] m_offsets =
        {
            new Vector3Pb {X = 0, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 0, Z = 50},
            new Vector3Pb {X = 50, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 50, Z = 0},
        };

        private readonly Vector3Pb[] m_normals =
        {
            new Vector3Pb {X = 0, Y = 1, Z = 0},
            new Vector3Pb {X = 1, Y = 0, Z = 0},
            new Vector3Pb {X = 0, Y = 0, Z = 1},
            new Vector3Pb {X = 1, Y = 0, Z = 1},
            new Vector3Pb {X = 0, Y = 1, Z = 0},
        };

        public InfinitePlanesTests()
        {
            m_planes = Enumerable.Range(0, 5).Select(id => new InfinitePlanePb()
            {
                Id = id,
                Message = $"{id}",
                Color = m_colors[id],
                Normal = m_normals[id],
                Offset = m_offsets[id],
            }).ToArray();
        }
        
        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            packet.InfinitePlanes.Data.Add(m_planes);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };
            m_planes[0].Color = new ColorPb{B = 128, G = 128, R = 128};
            m_planes[2].Offset = new Vector3Pb {X = 0, Y = 0, Z = -50};
            m_planes[4].Normal = new Vector3Pb {X = 1, Y = 1, Z = 1};

            packet.InfinitePlanes.Data.Add(m_planes);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };

            packet.InfinitePlanes.Data.Add(new[] { m_planes[1], m_planes[3] });

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                InfinitePlanes = new PacketPb.Types.InfinitePlanes(),
            };

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}