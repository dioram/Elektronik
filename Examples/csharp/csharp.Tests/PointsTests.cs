using Elektronik.Common.Data.Pb;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace csharp.Tests
{
    public class PointsTests : TestsBase
    {
        private PointPb[] m_map;
        private ConnectionPb[] m_connections;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            m_map = Enumerable.Range(0, 5).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
            }).ToArray();

            m_map[0].Position = new Vector3Pb() { X = 0, Y = .5 };
            m_map[1].Position = new Vector3Pb() { X = .5, Y = -.5 };
            m_map[2].Position = new Vector3Pb() { X = -.5, Y = -.5 };
            m_map[3].Position = new Vector3Pb() { X = -.5, Y = 0 };
            m_map[4].Position = new Vector3Pb() { X = .5, Y = 0 };

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
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(m_map);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            m_map[0].Position.Z += .5;
            m_map[2].Position.Z += .5;
            m_map[4].Position.Z += .5;

            packet.Points.Data.Add(m_map);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void UpdateConnections()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            packet.Connections.Data.Add(m_connections);
            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4)]
        public void RemoveConnections()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections()
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
            packet.Connections.Data.Add(new[] { m_connections[0], m_connections[1] });
            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
            };

            packet.Points.Data.Add(new[] { m_map[1], m_map[3] });

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(6)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            };

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}