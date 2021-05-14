using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class ObservationsTests : TestsBase
    {
        private readonly ObservationPb[] _map;
        private readonly ConnectionPb[] _connections;

        private static readonly string Filename = $"{nameof(ObservationsTests)}.dat";
        private static int _timestamp;

        public ObservationsTests()
        {
            _map = Enumerable.Range(0, 5).Select(id => new ObservationPb
            {
                Point = new PointPb()
                {
                    Id = id,
                    Message = $"{id}",
                },
                Message = $"Observation #{id}",
                Filename = $"{id}.png",
            }).ToArray();

            _map[0].Point.Position = new Vector3Pb {X = 0, Y = .5};
            _map[1].Point.Position = new Vector3Pb {X = .5, Y = -.5};
            _map[2].Point.Position = new Vector3Pb {X = -.5, Y = -.5};
            _map[3].Point.Position = new Vector3Pb {X = -.5, Y = 0};
            _map[4].Point.Position = new Vector3Pb {X = .5, Y = 0};
            _map[0].Point.Color = new ColorPb{R = 255};
            _map[1].Point.Color = new ColorPb {G = 255};
            _map[2].Point.Color = new ColorPb {B = 255};
            _map[3].Point.Color = new ColorPb {R = 255, G = 255};
            _map[4].Point.Color = new ColorPb {R = 255, B = 255};

            _connections = new[]
            {
                new ConnectionPb {Id1 = _map[0].Point.Id, Id2 = _map[1].Point.Id,},
                new ConnectionPb {Id1 = _map[0].Point.Id, Id2 = _map[2].Point.Id,},
                new ConnectionPb {Id1 = _map[2].Point.Id, Id2 = _map[4].Point.Id,},
                new ConnectionPb {Id1 = _map[1].Point.Id, Id2 = _map[3].Point.Id,},
                new ConnectionPb {Id1 = _map[3].Point.Id, Id2 = _map[4].Point.Id,},
            };
        }

        [Test, Order(1), Explicit]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            packet.Observations.Data.Add(_map);

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
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            _map[0].Point.Position.Z += .5;
            _map[0].Point.Color = new ColorPb {R = 255};
            _map[0].Filename = "";
            _map[2].Point.Position.Z += .5;
            _map[2].Point.Color = new ColorPb {B = 255};
            _map[2].Filename = "";
            _map[4].Point.Position.Z += .5;
            _map[4].Point.Color = new ColorPb {R = 255, B = 255};
            _map[4].Filename = "";
            _map[0].Orientation = new Vector4Pb {X = 0, Y = .5, Z = 0, W = 1};
            _map[1].Orientation = new Vector4Pb {X = 0, Y = .5, Z = 0, W = 1};
            _map[2].Orientation = new Vector4Pb {X = 0, Y = .5, Z = 0, W = 1};
            _map[3].Orientation = new Vector4Pb {X = 0, Y = .5, Z = 0, W = 1};
            _map[4].Orientation = new Vector4Pb {X = 0, Y = .5, Z = 0, W = 1};

            foreach (var pb in _map)
            {
                pb.Message = $"{pb.Point.Position.X}";
            }

            packet.Observations.Data.Add(_map);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(3), Explicit]
        public void UpdateConnections()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
                Timestamp = ++_timestamp,
            };
            packet.Connections.Data.Add(_connections);

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }

        [Test, Order(4), Explicit]
        public void RemoveConnections()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
                Timestamp = ++_timestamp,
            };
            packet.Connections.Data.Add(new[] {_connections[0], _connections[1]});

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
                Action = PacketPb.Types.ActionType.Remove,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };

            packet.Observations.Data.Add(new[] {_map[1], _map[3]});

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
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };

            using var file = File.Open(Filename, FileMode.Append);
            packet.WriteDelimitedTo(file);

            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}