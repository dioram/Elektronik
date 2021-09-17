using System;
using System.Linq;
using System.Threading;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class PlanesTests : OnlineTestsBase
    {
        private PlanePb[] _planes;

        private readonly Vector3Pb[] _offsets =
        {
            new() { X = 0, Y = 0, Z = 0 },
            new() { X = 0, Y = 0, Z = 0 },
            new() { X = 0, Y = 0, Z = 50 },
            new() { X = 50, Y = 0, Z = 0 },
            new() { X = 0, Y = 50, Z = 0 },
        };

        private readonly Vector3Pb[] _normals =
        {
            new() { X = 0, Y = 1, Z = 0 },
            new() { X = 1, Y = 0, Z = 0 },
            new() { X = 0, Y = 0, Z = 1 },
            new() { X = 1, Y = 0, Z = 1 },
            new() { X = 0, Y = 1, Z = 0 },
        };

        public PlanesTests() : base(40013)
        {
            _planes = Enumerable.Range(0, 5).Select(i => new PlanePb
            {
                Id = i,
                Message = $"{i}",
                Color = new ColorPb { R = i * 50 },
                Normal = _normals[i],
                Offset = _offsets[i],
            }).ToArray();
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(_planes);
            var e = new AddedEventArgs<SlamPlane>(_planes.Select(p => ((SlamPlaneDiff)p).Apply())
                                                      .ToArray());

            var response = MapClient.Handle(packet);
            Thread.Sleep(20);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(2)]
        public void UpdateOffsets()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            var newOffsets = new Vector3Pb[]
            {
                new() { X = 0, Y = 0, Z = 100 },
                new() { X = 0, Y = 100, Z = 100 },
                new() { X = 100, Y = 0, Z = 100 },
                new() { X = 0, Y = 100, Z = 100 },
                new() { X = 100, Y = 0, Z = 100 },
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PlanePb(pb) { Offset = newOffsets[i] };
                var diff = new PlanePb { Id = i, Offset = newOffsets[i] };
                return (newPb, diff);
            });
            packet.Planes.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => ((SlamPlaneDiff)p).Apply())
                                                                    .ToArray());

            var response = MapClient.Handle(packet);
            Thread.Sleep(20);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(3)]
        public void UpdateNormals()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            var newNormals = new Vector3Pb[]
            {
                new() { X = -10, Y = -10, Z = -10 },
                new() { X = -10, Y = -10, Z = -10 },
                new() { X = -10, Y = -10, Z = -50 },
                new() { X = -50, Y = -10, Z = -10 },
                new() { X = -10, Y = -50, Z = -10 },
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PlanePb(pb) { Normal = newNormals[i] };
                var diff = new PlanePb { Id = i, Normal = newNormals[i] };
                return (newPb, diff);
            });
            packet.Planes.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => ((SlamPlaneDiff)p).Apply())
                                                                    .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(4)]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PlanePb(pb) { Color = { R = 255, G = 255, B = 255 } };
                var diff = new PlanePb { Id = i, Color = new ColorPb { R = 255, B = 255, G = 255 } };
                return (newPb, diff);
            });
            packet.Planes.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => ((SlamPlaneDiff)p).Apply())
                                                                    .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(5)]
        public void ComplexUpdate()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            for (var i = 0; i < _planes.Length; i++)
            {
                _planes[i] = new PlanePb
                {
                    Id = i,
                    Offset = new Vector3Pb { X = i, Y = i, Z = i },
                    Normal = new Vector3Pb { X = 1 },
                    Color = new ColorPb { R = i * 50, G = i * 50, B = i * 50 },
                    Message = "Test message",
                };
            }

            packet.Planes.Data.Add(_planes);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => ((SlamPlaneDiff)p).Apply())
                                                                    .ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(6)]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(new[] { _planes[1], _planes[3] });
            var e = new RemovedEventArgs<SlamPlane>(new[]
            {
                ((SlamPlaneDiff) _planes[1]).Apply(),
                ((SlamPlaneDiff)_planes[3]).Apply(),
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length - 2);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(7)]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                Planes = new PacketPb.Types.Planes(),
            };
            var e = new RemovedEventArgs<SlamPlane>(new[]
            {
                ((SlamPlaneDiff) _planes[0]).Apply(),
                ((SlamPlaneDiff)_planes[2]).Apply(),
                ((SlamPlaneDiff)_planes[4]).Apply(),
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(0);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(8)]
        public void CheckCalls()
        {
            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(0);

            MockedPlanesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                    It.IsAny<AddedEventArgs<SlamPlane>>()),
                                                Times.Once);
            MockedPlanesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                      It.IsAny<UpdatedEventArgs<SlamPlane>>()),
                                                Times.Exactly(4));
            MockedPlanesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                      It.IsAny<RemovedEventArgs<SlamPlane>>()),
                                                Times.Exactly(2));


            MockedPointsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                              It.IsAny<UpdatedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(), 
                                                              It.IsAny<RemovedEventArgs<SlamPoint>>()),
                                        Times.Never);


            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SlamLine>>(),
                                                               It.IsAny<AddedEventArgs<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<UpdatedEventArgs<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs<SlamLine>>()),
                                           Times.Never);


            MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs<SimpleLine>>()),
                                             Times.Never);


            MockedObservationsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                  It.IsAny<AddedEventArgs<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                    It.IsAny<UpdatedEventArgs<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                    It.IsAny<RemovedEventArgs<SlamObservation>>()),
                                              Times.Never);


            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);


            MockedImageRenderer.Verify(r => r.Render(It.IsAny<byte[]>()), Times.Never);
            MockedImageRenderer.Verify(r => r.Clear(), Times.Never);
        }

        #region Not tests

        private PlanePb[] CreateDiff(Func<PlanePb, int, (PlanePb, PlanePb)> func)
        {
            var result = _planes.Select(func).ToArray();
            _planes = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}