using System.Collections.Generic;
using System.Threading;
using Elektronik;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Extensions;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online.GrpcServices;
using Elektronik.Protobuf.Recorders;
using Grpc.Core;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class RetranslatorTests
    {
        private const string Address = "127.0.0.1";
        private const int Port = 63784;
        private Server _grpcServer;
        private ProtobufRetranslator _retranslator;

        private Mock<IConnectableObjectsContainer<SlamPoint>> _mockedPointsContainer;
        private Mock<IConnectableObjectsContainer<SlamObservation>> _mockedObservationsContainer;
        private Mock<IContainer<SlamTrackedObject>> _mockedTrackedObjectsContainer;
        private Mock<IContainer<SlamLine>> _mockedLinesContainer;
        private Mock<IContainer<SlamInfinitePlane>> _mockedInfinitePlanesContainer;

        [SetUp]
        public void SetUp()
        {
            _mockedPointsContainer = new Mock<IConnectableObjectsContainer<SlamPoint>>();
            _mockedObservationsContainer = new Mock<IConnectableObjectsContainer<SlamObservation>>();
            _mockedTrackedObjectsContainer = new Mock<IContainer<SlamTrackedObject>>();
            _mockedLinesContainer = new Mock<IContainer<SlamLine>>();
            _mockedInfinitePlanesContainer = new Mock<IContainer<SlamInfinitePlane>>();
            _retranslator = new ProtobufRetranslator
            {
                Settings = new AddressesSettingsBag {Addresses = $"{Address}:{Port}"}
            };
            _retranslator.Start();

            var logger = new TestsLogger();
            var pointsMapManager = new PointsMapManager(_mockedPointsContainer.Object, null);
            var observationsMapManager = new ObservationsMapManager(_mockedObservationsContainer.Object, null);
            var trackedObjsMapManager = new TrackedObjsMapManager(_mockedTrackedObjectsContainer.Object, null);
            var linesMapManager = new LinesMapManager(_mockedLinesContainer.Object, null);
            var infinitePlanesMapManager = new InfinitePlanesMapManager(_mockedInfinitePlanesContainer.Object, null);
            pointsMapManager.Logger = logger;
            observationsMapManager.Logger = logger;
            trackedObjsMapManager.Logger = logger;
            linesMapManager.Logger = logger;
            infinitePlanesMapManager.Logger = logger;
            
            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                pointsMapManager, observationsMapManager, trackedObjsMapManager, linesMapManager,
                infinitePlanesMapManager
            }.BuildChain();
            GrpcEnvironment.SetLogger(new TestsLogger());

            _grpcServer = new Server
            {
                Services =
                {
                    MapsManagerPb.BindService(servicesChain),
                },
                Ports =
                {
                    new ServerPort(Address, Port, ServerCredentials.Insecure),
                },
            };
            _grpcServer.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _grpcServer.ShutdownAsync().Wait();
        }

        [Test]
        public void PointsTest()
        {
            var points = new[]
            {
                new SlamPoint(0, Vector3.down, Color.black),
                new SlamPoint(1, Vector3.forward, Color.blue),
            };
            var morePoints = new[]
            {
                new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                new SlamPoint(-10, Vector3.up, Color.white, "test message")
            };
            _retranslator.OnAdded("", points);
            _retranslator.OnUpdated("", morePoints);
            _retranslator.OnRemoved<SlamPoint>("", new List<int> {1});
            Thread.Sleep(100);
            _mockedPointsContainer.Verify(c => c.AddRange(points), Times.Once);
            _mockedPointsContainer.Verify(c => c.Update(morePoints), Times.Once);
            _mockedPointsContainer.Verify(c => c.Remove(It.IsAny<IEnumerable<SlamPoint>>()), Times.Once);
        }

        [Test]
        public void ObservationsTest()
        {
            var observations = new[]
            {
                new SlamObservation(new SlamPoint(0, Vector3.one, Color.blue), Quaternion.identity, "123", "f.jpg"),
                new SlamObservation(new SlamPoint(1, Vector3.down, Color.red), new Quaternion(0, 5, 15, 3), "321", ""),
                new SlamObservation(new SlamPoint(-10, Vector3.forward, Color.gray, "test message"),
                                    new Quaternion(59, 46, 3, 24), "adsf", "f.jpg"),
            };
            var moreObservations = new[]
            {
                new SlamObservation(new SlamPoint(0, Vector3.zero, Color.red, "Another message"),
                                    new Quaternion(6, 2, 1, 0), "123", "f.jpg"),
                new SlamObservation(new SlamPoint(-10, Vector3.up, Color.white, "test message"), Quaternion.identity,
                                    "123", "f.jpg"),
            };

            _retranslator.OnAdded("", observations);
            _retranslator.OnUpdated("", moreObservations);
            _retranslator.OnRemoved<SlamObservation>("", new List<int> {1});
            Thread.Sleep(100);
            _mockedObservationsContainer.Verify(c => c.AddRange(It.IsAny<IEnumerable<SlamObservation>>()), Times.Once);
            _mockedObservationsContainer.Verify(c => c.Update(It.IsAny<IEnumerable<SlamObservation>>()), Times.Once);
            _mockedObservationsContainer.Verify(c => c.Remove(It.IsAny<IEnumerable<SlamObservation>>()), Times.Once);
        }

        [Test]
        public void TrackedObjectsTest()
        {
            var trackedObjects = new[]
            {
                new SlamTrackedObject(0, Vector3.one, Quaternion.identity, Color.blue),
                new SlamTrackedObject(1, Vector3.down, new Quaternion(1, 2, 3, 1), Color.red),
                new SlamTrackedObject(-10, Vector3.forward, new Quaternion(3, 2, 1, 2), Color.gray, "test message")
            };
            var moreTrackedObjects = new[]
            {
                new SlamTrackedObject(0, Vector3.zero, new Quaternion(5, 6, 7, 0), Color.red, "Another message"),
                new SlamTrackedObject(-10, Vector3.up, Quaternion.identity, Color.white, "test message")
            };

            _retranslator.OnAdded("", trackedObjects);
            _retranslator.OnUpdated("", moreTrackedObjects);
            _retranslator.OnRemoved<SlamTrackedObject>("", new List<int> {1});
            Thread.Sleep(100);
            _mockedTrackedObjectsContainer.Verify(c => c.AddRange(It.IsAny<IEnumerable<SlamTrackedObject>>()),
                                                  Times.Once);
            _mockedTrackedObjectsContainer.Verify(c => c.Update(It.IsAny<IEnumerable<SlamTrackedObject>>()),
                                                  Times.Once);
            _mockedTrackedObjectsContainer.Verify(c => c.Remove(It.IsAny<IEnumerable<SlamTrackedObject>>()),
                                                  Times.Once);
        }

        [Test]
        public void LinesTest()
        {
            var lines = new[]
            {
                new SlamLine(0, 1),
                new SlamLine(1, 2),
                new SlamLine(-10, -6)
            };
            var moreLines = new[]
            {
                new SlamLine(0, 3),
                new SlamLine(-10, 4)
            };

            _retranslator.OnAdded("", lines);
            _retranslator.OnUpdated("", moreLines);
            _retranslator.OnRemoved<SlamLine>("", new List<int> {1});
            Thread.Sleep(100);
            _mockedLinesContainer.Verify(c => c.AddRange(It.IsAny<IEnumerable<SlamLine>>()), Times.Once);
            _mockedLinesContainer.Verify(c => c.Update(It.IsAny<IEnumerable<SlamLine>>()), Times.Once);
            _mockedLinesContainer.Verify(c => c.Remove(It.IsAny<IEnumerable<SlamLine>>()), Times.Once);
        }

        [Test]
        public void InfinitePlanesTest()
        {
            var planes = new[]
            {
                new SlamInfinitePlane(0, Vector3.one, Vector3.zero, Color.blue),
                new SlamInfinitePlane(1, Vector3.down, Vector3.up, Color.red),
                new SlamInfinitePlane(-10, Vector3.forward, Vector3.back, Color.gray, "test message")
            };
            var morePlanes = new[]
            {
                new SlamInfinitePlane(0, Vector3.zero, Vector3.left, Color.red, "Another message"),
                new SlamInfinitePlane(-10, Vector3.up, Vector3.right, Color.white, "test message")
            };

            _retranslator.OnAdded("", planes);
            _retranslator.OnUpdated("", morePlanes);
            _retranslator.OnRemoved<SlamInfinitePlane>("", new List<int> {1});
            Thread.Sleep(100);
            _mockedInfinitePlanesContainer.Verify(c => c.AddRange(It.IsAny<IEnumerable<SlamInfinitePlane>>()),
                                                  Times.Once);
            _mockedInfinitePlanesContainer.Verify(c => c.Update(It.IsAny<IEnumerable<SlamInfinitePlane>>()),
                                                  Times.Once);
            _mockedInfinitePlanesContainer.Verify(c => c.Remove(It.IsAny<IEnumerable<SlamInfinitePlane>>()),
                                                  Times.Once);
        }

        [Test]
        public void ConnectedPointsTest()
        {
            var connections = new List<(int, int)> {(0, 1), (1, 5), (3, 7), (6, 4)};
            var removedConnections = new List<(int, int)> {(1, 5), (3, 7)};
            _retranslator.OnConnectionsUpdated<SlamPoint>("", connections);
            _retranslator.OnConnectionsRemoved<SlamPoint>("", removedConnections);
            Thread.Sleep(100);
            _mockedPointsContainer.Verify(c => c.AddConnections(It.IsAny<IEnumerable<(int, int)>>()), Times.Once);
            _mockedPointsContainer.Verify(c => c.RemoveConnections(It.IsAny<IEnumerable<(int, int)>>()), Times.Once);
        }

        [Test]
        public void ConnectedObservationsTest()
        {
            var connections = new List<(int, int)> {(0, 1), (1, 5), (3, 7), (6, 4)};
            var removedConnections = new List<(int, int)> {(1, 5), (3, 7)};
            _retranslator.OnConnectionsUpdated<SlamObservation>("", connections);
            _retranslator.OnConnectionsRemoved<SlamObservation>("", removedConnections);
            Thread.Sleep(100);
            _mockedObservationsContainer.Verify(c => c.AddConnections(It.IsAny<IEnumerable<(int, int)>>()), Times.Once);
            _mockedObservationsContainer.Verify(c => c.RemoveConnections(It.IsAny<IEnumerable<(int, int)>>()),
                                                Times.Once);
        }
    }
}