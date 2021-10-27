using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Plugins.Common;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.Plugins.Common.DataDiff;
using Elektronik.Plugins.Common.FrameBuffers;
using Elektronik.Plugins.Common.Parsing;
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
        private const int Port = 45983;
        private Server _grpcServer;
        private ProtobufRetranslator _retranslator;
        private Mock<IOnlineFrameBuffer> _mockedBuffer;

        private Mock<IConnectableObjectsContainer<SlamPoint>> _mockedPointsContainer;
        private Mock<IConnectableObjectsContainer<SlamObservation>> _mockedObservationsContainer;
        private Mock<IContainer<SlamTrackedObject>> _mockedTrackedObjectsContainer;
        private Mock<IContainer<SlamPlane>> _mockedPlanesContainer;

        [SetUp]
        public void Setup()
        {
            _mockedBuffer = new Mock<IOnlineFrameBuffer>();
            _mockedPointsContainer = new Mock<IConnectableObjectsContainer<SlamPoint>>();
            _mockedObservationsContainer = new Mock<IConnectableObjectsContainer<SlamObservation>>();
            _mockedTrackedObjectsContainer = new Mock<IContainer<SlamTrackedObject>>();
            _mockedPlanesContainer = new Mock<IContainer<SlamPlane>>();

            var factory = new ProtobufRetranslatorFactory();
            _retranslator = (ProtobufRetranslator)factory.Start();
            var settings = (RetranslatorSettingsBag)_retranslator.Settings;
            settings.Addresses = $"{Address}:{Port}";
            settings.StartRetranslation?.Invoke();

            var converter = new RightHandToLeftHandConverter();

            var logger = new TestsLogger();
            var pointsMapManager = new PointsMapManager(_mockedBuffer.Object, _mockedPointsContainer.Object,
                                                        converter, logger);
            var observationsMapManager = new ObservationsMapManager(_mockedBuffer.Object,
                                                                    _mockedObservationsContainer.Object,
                                                                    converter, logger);
            var trackedObjsMapManager = new TrackedObjsMapManager(_mockedBuffer.Object,
                                                                  _mockedTrackedObjectsContainer.Object,
                                                                  converter, logger);
            var planesMapManager = new PlanesMapManager(_mockedBuffer.Object, _mockedPlanesContainer.Object,
                                                        converter, logger);

            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                pointsMapManager,
                observationsMapManager,
                trackedObjsMapManager,
                planesMapManager
            }.BuildChain();
            GrpcEnvironment.SetLogger(new TestsLogger());

            _grpcServer = new Server
            {
                Services = { MapsManagerPb.BindService(servicesChain), },
                Ports = { new ServerPort(Address, Port, ServerCredentials.Insecure), },
            };
            _grpcServer.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _mockedBuffer.Reset();
            var settings = (RetranslatorSettingsBag)_retranslator.Settings;
            settings.StopRetranslation?.Invoke();
            _grpcServer.ShutdownAsync().Wait();
        }

        [Test, Order(1)]
        public void PointsTest()
        {
            var added = new[]
            {
                new SlamPointDiff(0, Vector3.down, Color.black),
                new SlamPointDiff(1, Vector3.forward, Color.blue),
            };
            var updated = new[]
            {
                new SlamPointDiff(0, Vector3.zero, Color.red, "Another message"),
                new SlamPointDiff(1, Vector3.up, Color.white, "test message")
            };
            var removed = new[] { new SlamPointDiff(1), };

            var addCommand = new AddCommand<SlamPoint, SlamPointDiff>(_mockedPointsContainer.Object, added);
            var updateCommand = new UpdateCommand<SlamPoint, SlamPointDiff>(_mockedPointsContainer.Object, updated);
            var removeCommand = new RemoveCommand<SlamPoint, SlamPointDiff>(_mockedPointsContainer.Object, removed);

            _retranslator.OnItemsAdded(null, new AddedEventArgs<SlamPoint>(added.Select(p => p.Apply()).ToArray()));
            Thread.Sleep(50);
            _retranslator.OnItemsUpdated(
                null, new UpdatedEventArgs<SlamPoint>(updated.Select(p => p.Apply()).ToArray()));
            Thread.Sleep(50);
            _retranslator.OnItemsRemoved(null, new RemovedEventArgs<SlamPoint>(new List<int> { 1 }));
            Thread.Sleep(100);

            _mockedBuffer.Verify(b => b.Add(addCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(updateCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(removeCommand, It.IsAny<DateTime>(), false), Times.Once);
        }

        [Test, Order(2)]
        public void ObservationsTest()
        {
            var added = new[]
            {
                new SlamObservationDiff(new SlamPointDiff(0, Vector3.one, Color.blue),
                                        Quaternion.identity, Array.Empty<int>(), "123",
                                        Path.Combine(Directory.GetCurrentDirectory(), "f.png")),
                new SlamObservationDiff(new SlamPointDiff(1, Vector3.down, Color.red),
                                        new Quaternion(0, 5, 15, 3), Array.Empty<int>(), "321"),
                new SlamObservationDiff(new SlamPointDiff(-10, Vector3.forward, Color.gray, "test message"),
                                        new Quaternion(59, 46, 3, 24), Array.Empty<int>(), "adsf",
                                        Path.Combine(Directory.GetCurrentDirectory(), "f.png")),
            };
            var updated = new[]
            {
                new SlamObservationDiff(new SlamPointDiff(0, Vector3.zero, Color.red, "Another message"),
                                        new Quaternion(6, 2, 1, 0), Array.Empty<int>(), "123",
                                        Path.Combine(Directory.GetCurrentDirectory(), "f.png")),
                new SlamObservationDiff(new SlamPointDiff(-10, Vector3.up, Color.white, "test message"),
                                        Quaternion.identity, Array.Empty<int>(), "123",
                                        Path.Combine(Directory.GetCurrentDirectory(), "f.png")),
            };
            var removed = new[] { new SlamObservationDiff(new SlamPointDiff(0)), };
            var addCommand = new AddCommand<SlamObservation, SlamObservationDiff>(
                _mockedObservationsContainer.Object, added);
            var updateCommand = new UpdateCommand<SlamObservation, SlamObservationDiff>(
                _mockedObservationsContainer.Object, updated);
            var removeCommand = new RemoveCommand<SlamObservation, SlamObservationDiff>(
                _mockedObservationsContainer.Object, removed);

            _retranslator.OnItemsAdded(
                null, new AddedEventArgs<SlamObservation>(added.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(100);
            _retranslator.OnItemsUpdated(
                null, new UpdatedEventArgs<SlamObservation>(updated.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(100);
            _retranslator.OnItemsRemoved(
                null, new RemovedEventArgs<SlamObservation>(removed.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(200);

            _mockedBuffer.Verify(b => b.Add(addCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(updateCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(removeCommand, It.IsAny<DateTime>(), false), Times.Once);
        }

        [Test, Order(3)]
        public void TrackedObjectsTest()
        {
            var added = new[]
            {
                new SlamTrackedObjectDiff(0, Vector3.one, Quaternion.identity, Color.blue),
                new SlamTrackedObjectDiff(1, Vector3.down, new Quaternion(1, 2, 3, 1), Color.red),
                new SlamTrackedObjectDiff(-10, Vector3.forward, new Quaternion(3, 2, 1, 2), Color.gray, "test message"),
            };
            var updated = new[]
            {
                new SlamTrackedObjectDiff(0, Vector3.zero, new Quaternion(5, 6, 7, 0), Color.red, "Another message"),
                new SlamTrackedObjectDiff(-10, Vector3.up, Quaternion.identity, Color.white, "test message"),
            };
            var removed = new[] { new SlamTrackedObjectDiff(0), };
            var addCommand =
                    new AddCommand<SlamTrackedObject, SlamTrackedObjectDiff>(
                        _mockedTrackedObjectsContainer.Object, added);
            var updateCommand =
                    new UpdateCommand<SlamTrackedObject, SlamTrackedObjectDiff>(
                        _mockedTrackedObjectsContainer.Object, updated);
            var removeCommand =
                    new RemoveCommand<SlamTrackedObject, SlamTrackedObjectDiff>(
                        _mockedTrackedObjectsContainer.Object, removed);

            _retranslator.OnItemsAdded(
                null, new AddedEventArgs<SlamTrackedObject>(added.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(50);
            _retranslator.OnItemsUpdated(
                null, new UpdatedEventArgs<SlamTrackedObject>(updated.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(50);
            _retranslator.OnItemsRemoved(
                null, new RemovedEventArgs<SlamTrackedObject>(removed.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(100);

            _mockedBuffer.Verify(b => b.Add(addCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(updateCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(removeCommand, It.IsAny<DateTime>(), false), Times.Once);
        }

        [Test, Order(4)]
        public void InfinitePlanesTest()
        {
            var added = new[]
            {
                new SlamPlaneDiff(0, Vector3.one, Vector3.zero, Color.blue),
                new SlamPlaneDiff(1, Vector3.down, Vector3.up, Color.red),
                new SlamPlaneDiff(-10, Vector3.forward, Vector3.back, Color.gray, "test message")
            };
            var updated = new[]
            {
                new SlamPlaneDiff(0, Vector3.zero, Vector3.left, Color.red, "Another message"),
                new SlamPlaneDiff(-10, Vector3.up, Vector3.right, Color.white, "test message")
            };
            var removed = new[] { new SlamPlaneDiff(0), };
            var addCommand = new AddCommand<SlamPlane, SlamPlaneDiff>(_mockedPlanesContainer.Object, added);
            var updateCommand = new UpdateCommand<SlamPlane, SlamPlaneDiff>(_mockedPlanesContainer.Object, updated);
            var removeCommand = new RemoveCommand<SlamPlane, SlamPlaneDiff>(_mockedPlanesContainer.Object, removed);

            _retranslator.OnItemsAdded(null, new AddedEventArgs<SlamPlane>(added.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(50);
            _retranslator.OnItemsUpdated(
                null, new UpdatedEventArgs<SlamPlane>(updated.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(50);
            _retranslator.OnItemsRemoved(
                null, new RemovedEventArgs<SlamPlane>(removed.Select(o => o.Apply()).ToArray()));
            Thread.Sleep(100);

            _mockedBuffer.Verify(b => b.Add(addCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(updateCommand, It.IsAny<DateTime>(), false), Times.Once);
            _mockedBuffer.Verify(b => b.Add(removeCommand, It.IsAny<DateTime>(), false), Times.Once);
        }
    }
}