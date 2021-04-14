using NUnit.Framework;
using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;

namespace Protobuf.Tests
{
    public class ObservationsTests : TestsBase
    {
        private ObservationPb[] m_map;
        private ConnectionPb[] m_connections;
        
        private string filename = $"{nameof(ObservationsTests)}.dat";
        private static int _timestamp;

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
                Timestamp = ++_timestamp,
            };
            var obs = Enumerable.Range(0, 5).Select(id => new ObservationPb()
            {
                Point = new PointPb()
                {
                    Id = id,
                    Message = $"{id}",
                },
                Message = $"Observation #{id}",
                Filename = $"{id}.png",
            }).ToArray();
            obs[0].Point.Position = new Vector3Pb() { X = 0, Y = .5 };
            obs[1].Point.Position = new Vector3Pb() { X = .5, Y = -.5 };
            obs[2].Point.Position = new Vector3Pb() { X = -.5, Y = -.5 };
            obs[3].Point.Position = new Vector3Pb() { X = -.5, Y = 0 };
            obs[4].Point.Position = new Vector3Pb() { X = .5, Y = 0 };
            packet.Observations.Data.Add(obs);
            
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
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            m_map[0].Point.Position.Z += .5;
            m_map[2].Point.Position.Z += .5;
            m_map[4].Point.Position.Z += .5;

            foreach (var pb in m_map)
            {
                pb.Message = $"{pb.Point.Position.X}";
            }

            packet.Observations.Data.Add(m_map);
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
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
                Timestamp = ++_timestamp,
            };
            packet.Connections.Data.Add(m_connections);
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            
            var response = MapClient.Handle(packet);
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
                Timestamp = ++_timestamp,
            };
            packet.Connections.Data.Add(new[] { m_connections[0], m_connections[1] });
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);
            
            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(5)]
        public void Remove()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Remove,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };

            packet.Observations.Data.Add(new[] { m_map[1], m_map[3] });
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(6)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            
            using var file = File.Open(filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}