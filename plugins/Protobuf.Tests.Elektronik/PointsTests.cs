using System.Linq;
using Elektronik.Protobuf.Data;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class PointsTests : TestsBase
    {
        private readonly PointPb[] _map;
        private readonly ConnectionPb[] _connections;
        private static readonly string Filename = $"{nameof(PointsTests)}.dat";

        public PointsTests()
        {
            _map = Enumerable.Range(0, 5).Select(id => new PointPb()
            {
                Id = id,
                Message = $"{id}",
            }).ToArray();

            _map[0].Position = new Vector3Pb {X = 0, Y = .5};
            _map[0].Color = new ColorPb {R = 255};
            _map[1].Position = new Vector3Pb {X = .5, Y = -.5};
            _map[1].Color = new ColorPb {G = 255};
            _map[2].Position = new Vector3Pb {X = -.5, Y = -.5};
            _map[2].Color = new ColorPb {B = 255};
            _map[3].Position = new Vector3Pb {X = -.5, Y = 0};
            _map[3].Color = new ColorPb {R = 255, G = 255};
            _map[4].Position = new Vector3Pb {X = .5, Y = 0};
            _map[4].Color = new ColorPb {R = 255, B = 255};

            _connections = new[]
            {
                new ConnectionPb {Id1 = _map[0].Id, Id2 = _map[1].Id,},
                new ConnectionPb {Id1 = _map[0].Id, Id2 = _map[2].Id,},
                new ConnectionPb {Id1 = _map[2].Id, Id2 = _map[4].Id,},
                new ConnectionPb {Id1 = _map[1].Id, Id2 = _map[3].Id,},
                new ConnectionPb {Id1 = _map[3].Id, Id2 = _map[4].Id,},
            };
        }

        [Test, Order(1), Explicit]
        public void Create()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(_map);

            SendAndCheck(packet, Filename);
        }
        
        [Test, Order(2), Explicit]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            
            packet.Points.Data.Add(new PointPb{Id = 0, Position = new Vector3Pb {X = 0.5f}});
            packet.Points.Data.Add(new PointPb{Id = 1, Position = new Vector3Pb {X = 1f}});
            packet.Points.Data.Add(new PointPb{Id = 2, Position = new Vector3Pb {X = 1.5f}});
            packet.Points.Data.Add(new PointPb{Id = 3, Position = new Vector3Pb {X = 2f}});
            packet.Points.Data.Add(new PointPb{Id = 4, Position = new Vector3Pb {X = 2.5f}});

            SendAndCheck(packet, Filename);
        }
        
        [Test, Order(3), Explicit]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            
            packet.Points.Data.Add(new PointPb{Id = 0, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Points.Data.Add(new PointPb{Id = 1, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Points.Data.Add(new PointPb{Id = 2, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Points.Data.Add(new PointPb{Id = 3, Color = new ColorPb {R = 255, G = 255, B = 255}});
            packet.Points.Data.Add(new PointPb{Id = 4, Color = new ColorPb {R = 255, G = 255, B = 255}});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(4), Explicit]
        public void UpdateConnections()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(5), Explicit]
        public void RemoveConnections()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
            
            SendAndCheck(packet, Filename);
        }

        [Test, Order(6), Explicit]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
            };

            packet.Points.Data.Add(new[] {_map[1], _map[3]});

            SendAndCheck(packet, Filename);
        }

        [Test, Order(7), Explicit]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Special = true,
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            };

            SendAndCheck(packet, Filename);
        }
    }
}