using Elektronik.Common.Data.Pb;
using NUnit.Framework;
using System;
using System.Linq;


namespace csharp.Tests
{
    public class ObservationsTests : TestsBase
    {
        private ObservationPb[] m_map;
        private ConnectionPb[] m_connections;

        public ObservationsTests()
        {
            m_map = Enumerable.Range(0, 5).Select(id => new ObservationPb()
            {
                Point = new PointPb() 
                {
                    Id = id,
                    Message = $"{id}",
                }
            }).ToArray();

            m_map[0].Point.Position = new Vector3Pb() { X = 0, Y = .5 };
            m_map[1].Point.Position = new Vector3Pb() { X = .5, Y = -.5 };
            m_map[2].Point.Position = new Vector3Pb() { X = -.5, Y = -.5 };
            m_map[3].Point.Position = new Vector3Pb() { X = -.5, Y = 0 };
            m_map[4].Point.Position = new Vector3Pb() { X = .5, Y = 0 };

            m_connections = new[]
            {
                new ConnectionPb() { Id1 = m_map[0].Point.Id, Id2 = m_map[1].Point.Id, },
                new ConnectionPb() { Id1 = m_map[0].Point.Id, Id2 = m_map[2].Point.Id, },
                new ConnectionPb() { Id1 = m_map[2].Point.Id, Id2 = m_map[4].Point.Id, },
                new ConnectionPb() { Id1 = m_map[1].Point.Id, Id2 = m_map[3].Point.Id, },
                new ConnectionPb() { Id1 = m_map[3].Point.Id, Id2 = m_map[4].Point.Id, },
            };
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
            };
            var obs = Enumerable.Range(0, 5).Select(id => new ObservationPb()
            {
                Point = new PointPb()
                {
                    Id = id,
                    Message = $"{id}",
                }
            }).ToArray();
            obs[0].Point.Position = new Vector3Pb() { X = 0, Y = .5 };
            obs[1].Point.Position = new Vector3Pb() { X = .5, Y = -.5 };
            obs[2].Point.Position = new Vector3Pb() { X = -.5, Y = -.5 };
            obs[3].Point.Position = new Vector3Pb() { X = -.5, Y = 0 };
            obs[4].Point.Position = new Vector3Pb() { X = .5, Y = 0 };
            packet.Observations.Data.Add(obs);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(2)]
        public void Update()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            m_map[0].Point.Position.Z += .5;
            m_map[2].Point.Position.Z += .5;
            m_map[4].Point.Position.Z += .5;

            packet.Observations.Data.Add(m_map);

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3)]
        public void UpdateConnections()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
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
                Observations = new PacketPb.Types.Observations(),
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
                Observations = new PacketPb.Types.Observations(),
            };

            packet.Observations.Data.Add(new[] { m_map[1], m_map[3] });

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(6)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
            };

            var response = m_client.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}