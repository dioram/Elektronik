using System.IO;
using NUnit.Framework;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;

namespace Protobuf.Tests
{
    public class PointsTests : TestsBase
    {
        private PointPb[] m_map;
        private ConnectionPb[] m_connections;
        private readonly string _filename = $"{nameof(PointsTests)}.dat";

        public PointsTests()
        {
            m_map = Enumerable.Range(0, 5).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
            }).ToArray();

            m_map[0].Position = new Vector3Pb() { X = 0, Y = .5 };
            m_map[0].Color = new ColorPb {R = 255};
            m_map[1].Position = new Vector3Pb() { X = .5, Y = -.5 };
            m_map[1].Color = new ColorPb {G = 255};
            m_map[2].Position = new Vector3Pb() { X = -.5, Y = -.5 };
            m_map[2].Color = new ColorPb {B = 255};
            m_map[3].Position = new Vector3Pb() { X = -.5, Y = 0 };
            m_map[3].Color = new ColorPb {R = 255, G = 255};
            m_map[4].Position = new Vector3Pb() { X = .5, Y = 0 };
            m_map[4].Color = new ColorPb {R = 255, B = 255};

            m_connections = new[]
            {
                new ConnectionPb() { Id1 = m_map[0].Id, Id2 = m_map[1].Id, },
                new ConnectionPb() { Id1 = m_map[0].Id, Id2 = m_map[2].Id, },
                new ConnectionPb() { Id1 = m_map[2].Id, Id2 = m_map[4].Id, },
                new ConnectionPb() { Id1 = m_map[1].Id, Id2 = m_map[3].Id, },
                new ConnectionPb() { Id1 = m_map[3].Id, Id2 = m_map[4].Id, },
            };
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(m_map);

            var response = MapClient.Handle(packet);
            
            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            m_map[0].Position.Z += .5;
            m_map[0].Color = new ColorPb {R = 255};
            m_map[2].Position.Z += .5;
            m_map[2].Color = new ColorPb {B = 255};
            m_map[4].Position.Z += .5;
            m_map[4].Color = new ColorPb {R = 255, B = 255};

            packet.Points.Data.Add(m_map);

            var response = MapClient.Handle(packet);
            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void UpdateConnections()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            packet.Connections.Data.Add(m_connections);
            var response = MapClient.Handle(packet);
            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void RemoveConnections()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
            packet.Connections.Data.Add(new[] { m_connections[0], m_connections[1] });
            var response = MapClient.Handle(packet);
            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
            };

            packet.Points.Data.Add(new[] { m_map[1], m_map[3] });

            var response = MapClient.Handle(packet);
            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(6)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            };

            var response = MapClient.Handle(packet);
            using var file = File.Open(_filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}