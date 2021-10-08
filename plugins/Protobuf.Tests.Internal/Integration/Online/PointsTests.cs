using System;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class PointsTests : OnlineTestsBase
    {
        private PointPb[] _map;
        private readonly ConnectionPb[] _connections;

        public PointsTests() : base(40011)
        {
            _map = Enumerable.Range(0, 5).Select(i => new PointPb
            {
                Id = i,
                Position = new Vector3Pb { X = i },
                Color = new ColorPb { B = i * 50 },
                Message = $"{i}",
            }).ToArray();
            _connections = new[]
            {
                new ConnectionPb { Id1 = _map[0].Id, Id2 = _map[1].Id, },
                new ConnectionPb { Id1 = _map[0].Id, Id2 = _map[2].Id, },
                new ConnectionPb { Id1 = _map[2].Id, Id2 = _map[4].Id, },
                new ConnectionPb { Id1 = _map[1].Id, Id2 = _map[3].Id, },
                new ConnectionPb { Id1 = _map[3].Id, Id2 = _map[4].Id, },
            };
        }

        [Test, Order(1)]
        public void CreatePoints()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(_map);
            var e = new AddedEventArgs<SlamPoint>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            MockedPointsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
        }

        [Test, Order(2)]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PointPb(pb) { Position = { Z = i } };
                var diff = new PointPb { Id = i, Position = new Vector3Pb { X = i, Z = i } };
                return (newPb, diff);
            });
            packet.Points.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPoint>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
        }

        [Test, Order(3)]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PointPb(pb) { Color = new ColorPb { R = 255, G = 255, B = 255 } };
                var diff = new PointPb { Id = i, Color = new ColorPb { R = 255, G = 255, B = 255 } };
                return (newPb, diff);
            });
            packet.Points.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPoint>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
        }

        [Test, Order(4)]
        public void ComplexUpdate()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            for (var i = 0; i < _map.Length; i++)
            {
                _map[i] = new PointPb
                {
                    Id = i,
                    Position = new Vector3Pb { X = i, Y = i, Z = i * 5 },
                    Color = new ColorPb { R = i * 50, G = i * 50, B = i * 50 },
                    Message = "Test message",
                };
            }

            packet.Points.Data.Add(_map);
            var e = new UpdatedEventArgs<SlamPoint>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
        }

        [Test, Order(5)]
        public void UpdateConnections()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            packet.Connections.Data.Add(_connections);
            var lines = _connections.Select(c => (_map[c.Id1], _map[c.Id2]))
                    .Select(pair => (pair.Item1.ToUnity(Converter).Apply(), pair.Item2.ToUnity(Converter).Apply()))
                    .Select((pair, i) => new SlamLine(pair.Item1, pair.Item2, i))
                    .ToArray();
            var e = new AddedEventArgs<SlamLine>(lines);

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(_connections.Length);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SlamLine>>(), e), Times.Once);
        }

        [Test, Order(6)]
        public void UpdateConnectedPoints()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
            };
            _map[2].Position = new Vector3Pb { X = 1.5f, Y = 1 };
            _map[2].Color = new ColorPb { R = 255 };
            var diff = new PointPb
            {
                Id = 2,
                Position = new Vector3Pb { X = 1.5f, Y = 1 },
                Color = new ColorPb { R = 255 },
            };
            packet.Points.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPoint>(new[] { _map[2].ToUnity(Converter).Apply() });
            var el = new UpdatedEventArgs<SlamLine>(new[]
            {
                new SlamLine(_map[0].ToUnity(Converter).Apply(), _map[2].ToUnity(Converter).Apply(), 1),
                new SlamLine(_map[2].ToUnity(Converter).Apply(), _map[4].ToUnity(Converter).Apply(), 2)
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(_connections.Length);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<IContainer<SlamLine>>(), el), Times.Once);
        }

        [Test, Order(7)]
        public void RemoveConnections()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Remove,
                },
            };
            packet.Connections.Data.Add(new[] { _connections[0], _connections[1] });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(_connections.Length - 2);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(), 
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()), 
                                           Times.Once);
        }

        [Test, Order(8)]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Points = new PacketPb.Types.Points(),
            };
            packet.Points.Data.Add(new[] { _map[1], _map[3] });
            var e = new RemovedEventArgs<SlamPoint>(new[]
            {
                _map[1].ToUnity(Converter).Apply(),
                _map[3].ToUnity(Converter).Apply()
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(_map.Length - 2);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(1);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(), 
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()), 
                                           Times.Exactly(2));
        }

        [Test, Order(9)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            };
            var e = new RemovedEventArgs<SlamPoint>(new[]
            {
                _map[0].ToUnity(Converter).Apply(),
                _map[2].ToUnity(Converter).Apply(),
                _map[4].ToUnity(Converter).Apply(),
            });
            
            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(), 
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()), 
                                           Times.Exactly(3));
        }
        
        #region Not tests

        private PointPb[] CreateDiff(Func<PointPb, int, (PointPb, PointPb)> func)
        {
            var result = _map.Select(func).ToArray();
            _map = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}