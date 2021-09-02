using System;
using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class TrackedObjectsTests : OnlineTestsBase
    {
        private TrackedObjPb[] _objects;

        public TrackedObjectsTests() : base(40005)
        {
            _objects = Enumerable.Range(0, 3).Select(i => new TrackedObjPb()
            {
                Id = i,
                Orientation = new Vector4Pb { W = 1, },
                Position = new Vector3Pb { X = i + 1 },
                Color = new ColorPb { R = 127 * i },
                Message = $"{i}",
            }).ToArray();
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            packet.TrackedObjs.Data.Add(_objects);
            var e = new AddedEventArgs<SlamTrackedObject>(_objects.Select(p => ((SlamTrackedObjectDiff)p).Apply()).ToArray());
            var els = _objects.Select(o => new AddedEventArgs<SimpleLine>(FromPb(0, o))).ToArray();

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackContainer>(), el), Times.Once);
            }
        }

        [Test, Order(2)]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var oldObjects = _objects.ToArray();
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new TrackedObjPb(pb) { Position = { X = i + 1, Z = 0.5f } };
                var diff = new TrackedObjPb { Id = i, Position = new Vector3Pb { X = i + 1, Z = 0.5f } };
                return (newPb, diff);
            });
            packet.TrackedObjs.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(o => ((SlamTrackedObjectDiff)o).Apply()).ToArray());
            var els = _objects.Select((o, i) => new AddedEventArgs<SimpleLine>(FromPb(1, oldObjects[i], o))).ToArray();

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackContainer>(), el), Times.Once);
            }
        }

        [Test, Order(3)]
        public void UpdateOrientations()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new TrackedObjPb(pb) { Orientation = { X = 1, W = 1 } };
                var diff = new TrackedObjPb { Id = i, Orientation = new Vector4Pb { X = 1, W = 1 } };
                return (newPb, diff);
            });
            packet.TrackedObjs.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(o => ((SlamTrackedObjectDiff)o).Apply()).ToArray());
            var els = _objects.Select(o => new AddedEventArgs<SimpleLine>(FromPb(2, o))).ToArray();

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackContainer>(), el), Times.Once);
            }
        }

        [Test, Order(4)]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var oldObjects = _objects.ToArray();
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new TrackedObjPb(pb) { Color = { R = 255, G = 255, B = 255 } };
                var diff = new TrackedObjPb { Id = i, Color = new ColorPb { R = 255, G = 255, B = 255 } };
                return (newPb, diff);
            });
            packet.TrackedObjs.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(o => ((SlamTrackedObjectDiff)o).Apply()).ToArray());
            var els = _objects.Select((o, i) => new AddedEventArgs<SimpleLine>(FromPb(3, oldObjects[i], o))).ToArray();

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackContainer>(), el), Times.Once);
            }
        }

        [Test, Order(5)]
        public void UpdateMessage()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new TrackedObjPb(pb) { Message = "Test message" };
                var diff = new TrackedObjPb { Id = i, Message = "Test message" };
                return (newPb, diff);
            });
            packet.TrackedObjs.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(o => ((SlamTrackedObjectDiff)o).Apply()).ToArray());
            var els = _objects.Select(o => new AddedEventArgs<SimpleLine>(FromPb(4, o))).ToArray();

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackContainer>(), el), Times.Once);
            }
        }

        [Test, Order(6)]
        public void ComplexUpdate()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var oldObjects = _objects.ToArray();
            for (var i = 0; i < _objects.Length; i++)
            {
                _objects[i] = new TrackedObjPb
                {
                    Id = i,
                    Position = new Vector3Pb { Y = i },
                    Orientation = new Vector4Pb { X = 0, Y = 1, Z = 1, W = 1 },
                    Color = new ColorPb { R = 0, G = 127 * i, B = 0 },
                    Message = "i",
                };
            }

            packet.TrackedObjs.Data.Add(_objects);
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(o => ((SlamTrackedObjectDiff)o).Apply()).ToArray());
            var els = _objects.Select((o, i) => new AddedEventArgs<SimpleLine>(FromPb(5, oldObjects[i], o))).ToArray();

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackContainer>(), el), Times.Once);
            }
        }

        [Test, Order(7)]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            packet.TrackedObjs.Data.Add(new[] { _objects[1] });
            var e = new RemovedEventArgs(new[] { 1 });
            var el = new RemovedEventArgs(Enumerable.Range(0, 6).ToArray());

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length - 1);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<TrackContainer>(), el),
                                             Times.Once);
        }

        [Test, Order(8)]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            var e = new RemovedEventArgs(new[] { 0, 2 });
            var el = new RemovedEventArgs(Enumerable.Range(0, 6).ToArray());

            var response = MapClient.Handle(packet);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(0);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<TrackContainer>(), el),
                                             Times.Exactly(3));
        }

        [Test, Order(9)]
        public void CheckCalls()
        {
            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).InfinitePlanes.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(0);

            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SlamTrackedObject>>()),
                                             Times.Once);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SlamTrackedObject>>()),
                                             Times.Exactly(5));
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs>()),
                                             Times.Exactly(2));
            MockedTrackedObjsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(),
                                                              It.IsAny<IList<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SimpleLine>>()),
                                             Times.Exactly(6 * 3));
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs>()),
                                             Times.Exactly(3));
            MockedSimpleLinesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedPointsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                              It.IsAny<UpdatedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SlamLine>>(),
                                                               It.IsAny<AddedEventArgs<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<UpdatedEventArgs<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedObservationsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                  It.IsAny<AddedEventArgs<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                    It.IsAny<UpdatedEventArgs<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                    It.IsAny<RemovedEventArgs>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(),
                                                               It.IsAny<IList<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


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

        private static SimpleLine FromPb(int id, TrackedObjPb obj) =>
                new(id, (Vector3)obj.Position!, (Vector3)obj.Position!, (Color)obj.Color!);

        private static SimpleLine FromPb(int id, TrackedObjPb obj1, TrackedObjPb obj2) =>
                new(id, (Vector3)obj1.Position!, (Vector3)(obj2.Position ?? obj1.Position)!,
                    (Color)obj1.Color!, (Color)(obj2.Color ?? obj1.Color)!);

        private TrackedObjPb[] CreateDiff(Func<TrackedObjPb, int, (TrackedObjPb, TrackedObjPb)> func)
        {
            var result = _objects.Select(func).ToArray();
            _objects = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}