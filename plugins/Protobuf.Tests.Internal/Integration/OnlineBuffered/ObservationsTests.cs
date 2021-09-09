using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.OnlineBuffered
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
            var e = new AddedEventArgs<SlamObservation>(_map.Select(p => ((SlamObservationDiff)p).Apply()).ToArray());

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
                var newPb = new ObservationPb(pb) { Point = { Position = { X = i, Y = 0, Z = 0 } } };
                var diff = new ObservationPb { Point = new PointPb { Id = i, Position = new Vector3Pb { X = i } } };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(_map.Select(p => ((SlamObservationDiff)p).Apply()).ToArray());

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
                var diff = new ObservationPb { Point = new PointPb {Id = i}, Orientation = new Vector4Pb { X = 1, W = 1 } };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(_map.Select(p => ((SlamObservationDiff)p).Apply()).ToArray());

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
                var diff = new ObservationPb { Point = new PointPb { Id = i, Color = new ColorPb { R = 255, G = 255 } } };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(_map.Select(p => ((SlamObservationDiff)p).Apply()).ToArray());

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
                var newPb = new ObservationPb(pb) { Message = $"{i}", Filename = "tmp.png"};
                var diff = new ObservationPb { Point = new PointPb {Id = i}, Message = $"{i}", Filename = "tmp.png" };
                return (newPb, diff);
            });
            packet.Observations.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamObservation>(GetMapWithFullPath()
                                                                  .Select(p => ((SlamObservationDiff)p).Apply())
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
                    Orientation = new Vector4Pb {X = -1, W = 1},
                    Message = "Test message",
                    Filename = "test.png",
                };
            }
            packet.Observations.Data.Add(_map);
            var e = new UpdatedEventArgs<SlamObservation>(GetMapWithFullPath()
                                                                  .Select(p => ((SlamObservationDiff)p).Apply())
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
                    .Select(pair => (((SlamObservationDiff)pair.Item1).Apply(),
                                     ((SlamObservationDiff)pair.Item2).Apply()))
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
            var e = new UpdatedEventArgs<SlamObservation>(new[] { ((SlamObservationDiff)_map[2]).Apply() });
            var el = new UpdatedEventArgs<SlamLine>(new[]
            {
                new SlamLine(((SlamObservationDiff)_map[0]).Apply(), ((SlamObservationDiff)_map[2]).Apply(), 1),
                new SlamLine(((SlamObservationDiff)_map[2]).Apply(), ((SlamObservationDiff)_map[4]).Apply(), 2)
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
            var e = new RemovedEventArgs(new[] { 0, 1 });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(_connections.Length-2);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(), e), Times.Once);
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
            var e = new RemovedEventArgs(new[] { 1, 3 });
            var el = new RemovedEventArgs(new[] { 3, 4 });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(_map.Length-2);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(1);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(), el), Times.Once);
        }

        [Test, Order(11)]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                Observations = new PacketPb.Types.Observations(),
            };
            var e = new RemovedEventArgs(new[] { 0, 2, 4 });
            var el = new RemovedEventArgs(new[] { 2 });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(), el), Times.Once);
        }

        [Test, Order(12)]
        public void CheckCalls()
        {
            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).InfinitePlanes.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(0);
            
            MockedObservationsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                  It.IsAny<AddedEventArgs<SlamObservation>>()),
                                              Times.Once);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                    It.IsAny<UpdatedEventArgs<SlamObservation>>()),
                                              Times.Exactly(6));
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                    It.IsAny<RemovedEventArgs>()),
                                              Times.Exactly(2));
            MockedObservationsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(),
                                                               It.IsAny<IList<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SlamLine>>(),
                                                               It.IsAny<AddedEventArgs<SlamLine>>()),
                                           Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<UpdatedEventArgs<SlamLine>>()),
                                           Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs>()),
                                           Times.Exactly(3));
            MockedSlamLinesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedPointsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(
                r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SlamPoint>>()),
                Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);

            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(),
                                                              It.IsAny<IList<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedInfinitePlanesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                    It.IsAny<AddedEventArgs<SlamInfinitePlane>>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                      It.IsAny<UpdatedEventArgs<SlamInfinitePlane>>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                      It.IsAny<RemovedEventArgs>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(),
                                                                 It.IsAny<IList<SlamInfinitePlane>>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedImageRenderer.Verify(r => r.Render(It.IsAny<byte[]>()), Times.Never);
            MockedImageRenderer.Verify(r => r.Clear(), Times.Never);
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