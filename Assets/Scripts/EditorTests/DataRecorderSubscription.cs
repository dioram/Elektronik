using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests
{
    public class DataRecorderSubscription
    {
        [Test]
        public void SubscribePointCloudContainer()
        {
            var points = new List<ICloudItem>
            {
                new SlamPoint(0, Vector3.back, Color.black),
                new SlamPoint(1, Vector3.down, Color.blue),
            };
            var container = new CloudContainer<SlamPoint>();
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");
            
            Assert.AreEqual(3, DataRecorderSubscriber.Subscriptions[mockedRecorder.Object].SelectMany(s => s.Value).Count());

            container.AddRange(points.OfType<SlamPoint>());
            container.Update(points.OfType<SlamPoint>());
            container.Remove(points.OfType<SlamPoint>());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamPoint), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();

            container.AddRange(points.OfType<SlamPoint>());
            container.Update(points.OfType<SlamPoint>());
            container.Remove(points.OfType<SlamPoint>());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamPoint), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void SubscribeObservationCloudContainer()
        {
            var observations = new List<ICloudItem>
            {
                new SlamObservation(new SlamPoint(0, Vector3.back, Color.black), Quaternion.identity, "", ""),
                new SlamObservation(new SlamPoint(1, Vector3.down, Color.blue), Quaternion.identity, "", ""),
            };
            var container = new CloudContainer<SlamObservation>();
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");

            container.AddRange(observations.OfType<SlamObservation>());
            container.Update(observations.OfType<SlamObservation>());
            container.Remove(observations.OfType<SlamObservation>());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), observations), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), observations), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamObservation), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();

            container.AddRange(observations.OfType<SlamObservation>());
            container.Update(observations.OfType<SlamObservation>());
            container.Remove(observations.OfType<SlamObservation>());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), observations), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), observations), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamObservation), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void SubscribeTrackedCloudContainer()
        {
            var tracked = new List<ICloudItem>
            {
                new SlamTrackedObject(0, Vector3.back, Quaternion.identity),
                new SlamTrackedObject(1, Vector3.down, Quaternion.identity),
            };
            var container = new TrackedObjectsContainer();
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");

            container.AddRange(tracked.OfType<SlamTrackedObject>());
            container.Update(tracked.OfType<SlamTrackedObject>());
            container.Remove(tracked.OfType<SlamTrackedObject>());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamTrackedObject), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();

            container.AddRange(tracked.OfType<SlamTrackedObject>());
            container.Update(tracked.OfType<SlamTrackedObject>());
            container.Remove(tracked.OfType<SlamTrackedObject>());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), tracked), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamTrackedObject), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void SubscribeConnectableCloudContainer()
        {
            var container = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                       new SlamLinesContainer());
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            mockedRecorder.Object.SubscribeOn(container, "");

            var points = new List<ICloudItem>
            {
                new SlamPoint(0, Vector3.back, Color.black),
                new SlamPoint(1, Vector3.down, Color.blue),
            };
            var connections = new List<(int, int)> {(0, 1)};
            container.AddRange(points.OfType<SlamPoint>());
            container.Update(points.OfType<SlamPoint>());
            container.AddConnections(connections);
            container.RemoveConnections(connections);
            container.Remove(points.OfType<SlamPoint>());
            mockedRecorder.Verify(r => r.OnConnectionsUpdated(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnConnectionsRemoved(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamPoint), It.IsAny<List<int>>()),
                                   Times.Once());
            
            mockedRecorder.Object.UnsubscribeFromEverything();
            
            container.AddRange(points.OfType<SlamPoint>());
            container.Update(points.OfType<SlamPoint>());
            container.AddConnections(connections);
            container.RemoveConnections(connections);
            container.Remove(points.OfType<SlamPoint>());
            mockedRecorder.Verify(r => r.OnConnectionsUpdated(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnConnectionsRemoved(It.IsAny<string>(), connections), Times.Once());
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnUpdated(It.IsAny<string>(), points), Times.Once());
            mockedRecorder.Verify(r => r.OnRemoved(It.IsAny<string>(), typeof(SlamPoint), It.IsAny<List<int>>()),
                                   Times.Once());
        }

        [Test]
        public void FailedTest()
        {
            Assert.Fail();
        }
    }
}