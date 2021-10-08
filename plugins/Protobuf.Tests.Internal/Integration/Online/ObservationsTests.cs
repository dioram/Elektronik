using System;
using System.Collections.Generic;
using System.IO;
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
    public class ObservationsTests : OnlineTestsBase
    {
        private ObservationPb[] _map;
        private readonly ConnectionPb[] _connections;

        public ObservationsTests() : base(40012)
        {
            _map = Enumerable.Range(0, 5).Select(i => new ObservationPb
            {
                Point = new PointPb()
                {
                    Id = i,
                    Message = $"{i}",
                    Position = new Vector3Pb { Z = i },
                    Color = new ColorPb { R = i * 50 },
                },
                Message = $"Observation #{i}",
                Filename = Path.Combine(Directory.GetCurrentDirectory(), $"{i}.png"),
            }).ToArray();

            _connections = new[]
            {
                new ConnectionPb { Id1 = _map[0].Point.Id, Id2 = _map[1].Point.Id, },
                new ConnectionPb { Id1 = _map[0].Point.Id, Id2 = _map[2].Point.Id, },
                new ConnectionPb { Id1 = _map[2].Point.Id, Id2 = _map[4].Point.Id, },
                new ConnectionPb { Id1 = _map[1].Point.Id, Id2 = _map[3].Point.Id, },
                new ConnectionPb { Id1 = _map[3].Point.Id, Id2 = _map[4].Point.Id, },
            };
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
            };
            packet.Observations.Data.Add(_map);
            var e = new AddedEventArgs<SlamObservation>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
        }

        [Test, Order(2)]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new ObservationPb(pb) { Point = { Position = { X = i, Y = i, Z = i } } };
                var diff = new ObservationPb
                        { Point = new PointPb { Id = i, Position = new Vector3Pb { X = i, Y = i, Z = i } } };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
        }

        [Test, Order(3)]
        public void UpdateOrientations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new ObservationPb(pb) { Orientation = new Vector4Pb { X = 1, W = 1 } };
                var diff = new ObservationPb
                        { Point = new PointPb { Id = i }, Orientation = new Vector4Pb { X = 1, W = 1 } };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
        }

        [Test, Order(4)]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new ObservationPb(pb) { Point = { Color = { R = 255, G = 255 } } };
                var diff = new ObservationPb
                        { Point = new PointPb { Id = i, Color = new ColorPb { R = 255, G = 255 } } };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
        }

        [Test, Order(5)]
        public void UpdateMessages()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new ObservationPb(pb) { Message = $"{i}", Filename = "tmp.png" };
                var diff = new ObservationPb { Point = new PointPb { Id = i }, Message = $"{i}", Filename = "tmp.png" };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(GetMapWithFullPath()
                                                                  .Select(p => p.ToUnity(Converter).Apply())
                                                                  .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
        }

        [Test, Order(6)]
        public void ComplexUpdate()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            for (var i = 0; i < _map.Length; i++)
            {
                _map[i] = new ObservationPb
                {
                    Point = new PointPb
                    {
                        Id = i,
                        Position = new Vector3Pb { X = i, Y = i, Z = i },
                        Color = new ColorPb { R = i * 50, G = i * 50, B = i * 50 },
                        Message = "Test message",
                    },
                    Orientation = new Vector4Pb { X = -1, W = 1 },
                    Message = "Test message",
                    Filename = "test.png",
                };
            }

            packet.Observations.Data.Add(_map);
            var e = new UpdatedEventArgs<SlamObservation>(GetMapWithFullPath()
                                                                  .Select(p => p.ToUnity(Converter).Apply())
                                                                  .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
        }

        [Test, Order(7)]
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
            };
            packet.Connections.Data.Add(_connections);
            var lines = _connections.Select(c => (_map[c.Id1], _map[c.Id2]))
                    .Select(pair => (pair.Item1.ToUnity(Converter).Apply(),
                                     pair.Item2.ToUnity(Converter).Apply()))
                    .Select((pair, i) => new SlamLine(pair.Item1, pair.Item2, i))
                    .ToArray();
            var e = new AddedEventArgs<SlamLine>(lines);

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(_connections.Length);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SlamLine>>(), e), Times.Once);
        }

        [Test, Order(8)]
        public void UpdateConnectedObservations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
            };
            _map[2].Point.Position = new Vector3Pb { X = 1.5f, Y = 1 };
            _map[2].Point.Color = new ColorPb { R = 255 };
            packet.Observations.Data.Add(_map[2]);
            var e = new UpdatedEventArgs<SlamObservation>(new[] { _map[2].ToUnity(Converter).Apply() });
            var el = new UpdatedEventArgs<SlamLine>(new[]
            {
                new SlamLine(_map[0].ToUnity(Converter).Apply(), _map[2].ToUnity(Converter).Apply(), 1),
                new SlamLine(_map[2].ToUnity(Converter).Apply(), _map[4].ToUnity(Converter).Apply(), 2)
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(_connections.Length);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<IContainer<SlamLine>>(), el), Times.Once);
        }

        [Test, Order(9)]
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
            };
            packet.Connections.Data.Add(new[] { _connections[0], _connections[1] });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(_connections.Length - 2);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()),
                                           Times.Once);
        }

        [Test, Order(10)]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Observations = new PacketPb.Types.Observations(),
            };
            packet.Observations.Data.Add(new[] { _map[1], _map[3] });
            var e = new RemovedEventArgs<SlamObservation>(new[]
            {
                _map[1].ToUnity(Converter).Apply(),
                _map[3].ToUnity(Converter).Apply()
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length - 2);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(1);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()),
                                           Times.Exactly(2));
        }

        [Test, Order(11)]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
            };
            var e = new RemovedEventArgs<SlamObservation>(new[]
            {
                _map[0].ToUnity(Converter).Apply(),
                _map[2].ToUnity(Converter).Apply(),
                _map[4].ToUnity(Converter).Apply(),
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()),
                                           Times.Exactly(3));
        }

        #region Not tests

        private IEnumerable<ObservationPb> GetMapWithFullPath()
        {
            return _map.Select(o =>
            {
                o.Filename = Path.Combine(Directory.GetCurrentDirectory(), o.Filename);
                return o;
            });
        }

        private ObservationPb[] CreateDiff(Func<ObservationPb, int, (ObservationPb, ObservationPb)> func)
        {
            var result = _map.Select(func).ToArray();
            _map = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}