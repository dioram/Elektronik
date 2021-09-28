using System.IO;
using System.Linq;
using Elektronik.Protobuf.Data;
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
                Filename = Path.Combine(Directory.GetCurrentDirectory(), $"{id}.png"),
            }).ToArray();

            _map[0].Point.Position = new Vector3Pb {X = 0, Y = .5};
            _map[1].Point.Position = new Vector3Pb {X = .5, Y = -.5};
            _map[2].Point.Position = new Vector3Pb {X = -.5, Y = -.5};
            _map[3].Point.Position = new Vector3Pb {X = -.5, Y = 0};
            _map[4].Point.Position = new Vector3Pb {X = .5, Y = 0};
            _map[0].Point.Color = new ColorPb {R = 255};
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

            SendAndCheck(packet, Filename, true);
        }
        
        [Test, Order(2), Explicit]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 0, Position = new Vector3Pb{Z = 0.5}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 1, Position = new Vector3Pb{Z = 1}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 2, Position = new Vector3Pb{Z = 1.5}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 3, Position = new Vector3Pb{Z = 2}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 4, Position = new Vector3Pb{Z = 2.5}}});

            SendAndCheck(packet, Filename);
        }
        
        [Test, Order(3), Explicit]
        public void UpdateOrientations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 0}, Orientation = new Vector4Pb {W = 1, X = 1}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 1}, Orientation = new Vector4Pb {W = 1, X = 1}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 2}, Orientation = new Vector4Pb {W = 1, X = 1}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 3}, Orientation = new Vector4Pb {W = 1, X = 1}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 4}, Orientation = new Vector4Pb {W = 1, X = 1}});

            SendAndCheck(packet, Filename);
        }
        
        [Test, Order(4), Explicit]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 0, Color = new ColorPb{R = 255, G = 255, B = 255}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 1, Color = new ColorPb{R = 255, G = 255, B = 255}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 2, Color = new ColorPb{R = 255, G = 255, B = 255}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 3, Color = new ColorPb{R = 255, G = 255, B = 255}}});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 4, Color = new ColorPb{R = 255, G = 255, B = 255}}});

            SendAndCheck(packet, Filename);
        }
        
        [Test, Order(5), Explicit]
        public void UpdateMessages()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };
            
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 0}, Message = "0", Filename = ""});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 1}, Message = "1", Filename = ""});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 2}, Message = "2", Filename = ""});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 3}, Message = "3", Filename = ""});
            packet.Observations.Data.Add(new ObservationPb{Point = new PointPb{Id = 4}, Message = "4", Filename = ""});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(6), Explicit]
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

            SendAndCheck(packet, Filename);
        }

        [Test, Order(7), Explicit]
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

            SendAndCheck(packet, Filename);
        }

        [Test, Order(8), Explicit]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };

            packet.Observations.Data.Add(new[] {_map[1], _map[3]});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(9), Explicit]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
                Timestamp = ++_timestamp,
            };

            SendAndCheck(packet, Filename);
        }
    }
}