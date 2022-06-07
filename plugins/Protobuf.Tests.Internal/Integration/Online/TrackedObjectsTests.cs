﻿using System;
using System.Linq;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
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

        public TrackedObjectsTests() : base(40015)
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
            var e = new AddedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                  .ToArray());
            var els = _objects.Select(o => new AddedEventArgs<SimpleLine>(FromPb(0, o))).ToArray();

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
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
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                    .ToArray());
            var els = _objects.Select((o, i) => new AddedEventArgs<SimpleLine>(FromPb(1, oldObjects[i], o))).ToArray();

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
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
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                    .ToArray());
            var els = _objects.Select(o => new AddedEventArgs<SimpleLine>(FromPb(2, o))).ToArray();

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
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
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                    .ToArray());
            var els = _objects.Select((o, i) => new AddedEventArgs<SimpleLine>(FromPb(3, oldObjects[i], o))).ToArray();

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
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
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                    .ToArray());
            var els = _objects.Select(o => new AddedEventArgs<SimpleLine>(FromPb(4, o))).ToArray();

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
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
            var e = new UpdatedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                    .ToArray());
            var els = _objects.Select((o, i) => new AddedEventArgs<SimpleLine>(FromPb(5, oldObjects[i], o))).ToArray();

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
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
            var e = new RemovedEventArgs<SlamTrackedObject>(_objects[1].ToUnity(Converter).Apply());
            var el = new RemovedEventArgs<SimpleLine>(Enumerable
                                                              .Range(0, 6)
                                                              .Select(
                                                                  i => new SimpleLine(i, Vector3.zero, Vector3.zero))
                                                              .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(_objects.Length - 1);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<TrackCloudContainer>(), el),
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
            var e = new RemovedEventArgs<SlamTrackedObject>(new[]
            {
                _objects[0].ToUnity(Converter).Apply(), _objects[2].ToUnity(Converter).Apply()
            });
            var el = new RemovedEventArgs<SimpleLine>(Enumerable
                                                              .Range(0, 6)
                                                              .Select(
                                                                  i => new SimpleLine(i, Vector3.zero, Vector3.zero))
                                                              .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(0);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<TrackCloudContainer>(), el),
                                             Times.Exactly(3));
        }

        #region Not tests

        private SimpleLine FromPb(int id, TrackedObjPb obj) =>
                new(id, obj.Position!.ToUnity(Converter), obj.Position!.ToUnity(Converter), obj.Color!.ToUnity());

        private SimpleLine FromPb(int id, TrackedObjPb obj1, TrackedObjPb obj2) =>
                new(id, obj1.Position!.ToUnity(Converter), (obj2.Position ?? obj1.Position)!.ToUnity(Converter),
                    obj1.Color!.ToUnity(), (obj2.Color ?? obj1.Color)!.ToUnity());

        private TrackedObjPb[] CreateDiff(Func<TrackedObjPb, int, (TrackedObjPb, TrackedObjPb)> func)
        {
            var result = _objects.Select(func).ToArray();
            _objects = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}