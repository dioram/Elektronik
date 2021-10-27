using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class SceneTests : OnlineTestsBase
    {
        private readonly byte[] _imageData;
        private readonly PointPb[] _pointsMap;
        private readonly ObservationPb[] _observationsMap;
        private readonly ConnectionPb[] _connections;
        private readonly TrackedObjPb[] _objects;
        private readonly PlanePb[] _planes;

        public SceneTests() : base(40016)
        {
            _imageData = File.ReadAllBytes("1.png");
            _pointsMap = Enumerable.Range(0, 5).Select(i => new PointPb
            {
                Id = i,
                Position = new Vector3Pb { X = i },
                Color = new ColorPb { B = i * 50 },
                Message = $"{i}",
            }).ToArray();
            _observationsMap = Enumerable.Range(0, 5).Select(i => new ObservationPb
            {
                Point = _pointsMap[i],
                Message = $"Observation #{i}",
                Filename = Path.Combine(Directory.GetCurrentDirectory(), $"{i}.png"),
            }).ToArray();
            _connections = new[]
            {
                new ConnectionPb { Id1 = _pointsMap[0].Id, Id2 = _pointsMap[1].Id, },
                new ConnectionPb { Id1 = _pointsMap[0].Id, Id2 = _pointsMap[2].Id, },
                new ConnectionPb { Id1 = _pointsMap[2].Id, Id2 = _pointsMap[4].Id, },
                new ConnectionPb { Id1 = _pointsMap[1].Id, Id2 = _pointsMap[3].Id, },
                new ConnectionPb { Id1 = _pointsMap[3].Id, Id2 = _pointsMap[4].Id, },
            };
            _objects = Enumerable.Range(0, 3).Select(i => new TrackedObjPb()
            {
                Id = i,
                Orientation = new Vector4Pb { W = 1, },
                Position = new Vector3Pb { X = i + 1 },
                Color = new ColorPb { R = 127 * i },
                Message = $"{i}",
            }).ToArray();

            Vector3Pb[] offsets =
            {
                new() { X = 0, Y = 0, Z = 0 },
                new() { X = 0, Y = 0, Z = 0 },
                new() { X = 0, Y = 0, Z = 50 },
                new() { X = 50, Y = 0, Z = 0 },
                new() { X = 0, Y = 50, Z = 0 },
            };
            Vector3Pb[] normals =
            {
                new() { X = 0, Y = 1, Z = 0 },
                new() { X = 1, Y = 0, Z = 0 },
                new() { X = 0, Y = 0, Z = 1 },
                new() { X = 1, Y = 0, Z = 1 },
                new() { X = 0, Y = 1, Z = 0 },
            };

            _planes = Enumerable.Range(0, 5).Select(i => new PlanePb
            {
                Id = i,
                Message = $"{i}",
                Color = new ColorPb { R = i * 50 },
                Normal = normals[i],
                Offset = offsets[i],
            }).ToArray();
        }

        [Test, Order(1)]
        public void SendImage()
        {
            // Sending image
            var packet = new PacketPb
            {
                Image = new ImagePb { Bytes = ByteString.CopyFrom(_imageData, 0, _imageData.Length) }
            };

            var response = MapClient.Handle(packet);

            Thread.Sleep(200);
            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            MockedImageRenderer.Verify(r => r.Render(_imageData), Times.Once);
        }

        [Test, Order(2)]
        public void SendPointsAndConnections()
        {
            var pointsPacket = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Points = new PacketPb.Types.Points(),
            };
            pointsPacket.Points.Data.Add(_pointsMap);
            var e = new AddedEventArgs<SlamPoint>(_pointsMap.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(pointsPacket);

            MockedPointsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Points, e), Times.Once);

            var connectionsPacket = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Points = new PacketPb.Types.Points(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            connectionsPacket.Connections.Data.Add(_connections);
            var lines = _connections.Select(c => (_pointsMap[c.Id1], _pointsMap[c.Id2]))
                    .Select(pair => (pair.Item1.ToUnity(Converter).Apply(), pair.Item2.ToUnity(Converter).Apply()))
                    .Select((pair, i) => new SlamLine(pair.Item1, pair.Item2, i))
                    .ToArray();
            var e1 = new AddedEventArgs<SlamLine>(lines);

            SendPacket(connectionsPacket);

            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<ICloudContainer<SlamLine>>(), e1), Times.Once);
        }

        [Test, Order(3)]
        public void SendObservationsAndConnections()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Observations = new PacketPb.Types.Observations(),
            };
            packet.Observations.Data.Add(_observationsMap);
            var e = new AddedEventArgs<SlamObservation>(_observationsMap.Select(p => p.ToUnity(Converter).Apply())
                                                                .ToArray());

            SendPacket(packet);

            MockedObservationsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Observations, e),
                                              Times.Once);

            var connectionsPacket = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Observations = new PacketPb.Types.Observations(),
                Connections = new PacketPb.Types.Connections
                {
                    Action = PacketPb.Types.Connections.Types.Action.Add,
                },
            };
            connectionsPacket.Connections.Data.Add(_connections);
            var lines = _connections.Select(c => (_observationsMap[c.Id1], _observationsMap[c.Id2]))
                    .Select(pair => (pair.Item1.ToUnity(Converter).Apply(),
                                     pair.Item2.ToUnity(Converter).Apply()))
                    .Select((pair, i) => new SlamLine(pair.Item1.ToPoint(), pair.Item2.ToPoint(), i))
                    .ToArray();
            var el = new AddedEventArgs<SlamLine>(lines);

            SendPacket(connectionsPacket);

            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<ICloudContainer<SlamLine>>(), el),
                                           Times.Exactly(2));
        }

        [Test, Order(4)]
        public void SendTrackedObjects()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                TrackedObjs = new PacketPb.Types.TrackedObjs(),
            };
            packet.TrackedObjs.Data.Add(_objects);
            var e = new AddedEventArgs<SlamTrackedObject>(_objects.Select(p => p.ToUnity(Converter).Apply())
                                                                  .ToArray());
            var els = _objects
                    .Select(o => new AddedEventArgs<SimpleLine>(new SimpleLine(0, o.Position!.ToUnity(Converter),
                                                                               o.Position!.ToUnity(Converter),
                                                                               o.Color!.ToUnity())))
                    .ToArray();

            SendPacket(packet);

            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).TrackedObjs, e),
                                             Times.Once);
            foreach (var el in els)
            {
                MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<TrackCloudContainer>(), el), Times.Once);
            }
        }

        [Test, Order(5)]
        public void SendPlanes()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(_planes);
            var e = new AddedEventArgs<SlamPlane>(_planes.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            MockedPlanesRenderer.Verify(
                r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(6)]
        public void ClearAll()
        {
            var e1 = new RemovedEventArgs<SimpleLine>(new[] { 0 });
            var e3 = new RemovedEventArgs<SlamTrackedObject>(Enumerable.Range(0, 3).ToArray());
            var ep5 = new RemovedEventArgs<SlamPoint>(Enumerable.Range(0, 5).ToArray());
            var eo5 = new RemovedEventArgs<SlamObservation>(Enumerable.Range(0, 5).ToArray());
            var el5 = new RemovedEventArgs<SlamLine>(Enumerable.Range(0, 5).ToArray());
            var eip5 = new RemovedEventArgs<SlamPlane>(Enumerable.Range(0, 5).ToArray());

            var response = SceneClient.Clear(new Empty());
            Thread.Sleep(200);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            Sut.AmountOfFrames.Should().Be(0);
            MockedImageRenderer.Verify(r => r.Clear(), Times.Once);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Points, ep5),
                                        Times.Once);
            MockedObservationsRenderer.Verify(
                r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Observations, eo5),
                Times.Once);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<ICloudContainer<SlamLine>>(), el5),
                                           Times.Exactly(2));
            MockedPlanesRenderer.Verify(
                r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Planes, eip5),
                Times.Once);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).TrackedObjs, e3),
                                             Times.Once);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<TrackCloudContainer>(), e1),
                                             Times.Exactly(3));
        }

        [Test, Order(7)]
        public void DisposeTest()
        {
            var ep = new RemovedEventArgs<SlamPoint>(new List<int>());
            var eo = new RemovedEventArgs<SlamObservation>(new List<int>());
            var eip = new RemovedEventArgs<SlamPlane>(new List<int>());
            var etr = new RemovedEventArgs<SlamTrackedObject>(new List<int>());

            Sut.Dispose();

            MockedImageRenderer.Verify(r => r.Clear(), Times.Exactly(2));
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Points, ep),
                                        Times.Once);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Observations, eo),
                                              Times.Once);
            MockedPlanesRenderer.Verify(
                r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Planes, eip),
                Times.Once);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).TrackedObjs, etr),
                                             Times.Once);
        }
    }
}