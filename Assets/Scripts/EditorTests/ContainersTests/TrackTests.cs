using System.Collections.Generic;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests.ContainersTests
{
    public class TrackTests
    {
        [Test]
        public void TrackContainerAddPoint()
        {
            var container = new TrackContainer(null, new SlamTrackedObject());
            var mockedRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            container.SetRenderer(mockedRenderer.Object);
            
            container.AddPointToTrack(Vector3.zero, Color.black);
            var e1 = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.zero, Vector3.zero, Color.black));
            mockedRenderer.Verify(r => r.OnItemsAdded(container, e1), Times.Once());
            
            container.AddPointToTrack(Vector3.one, Color.red);
            var e2 = new AddedEventArgs<SimpleLine>(new SimpleLine(1, Vector3.zero, Vector3.one, Color.black, Color.red));
            mockedRenderer.Verify(r => r.OnItemsAdded(container, e2), Times.Once());
        }
        
        [Test]
        public void TrackContainerAddSlamPoint()
        {
            var container = new TrackContainer(null, new SlamTrackedObject());
            var mockedRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            container.SetRenderer(mockedRenderer.Object);
            
            container.AddPointToTrack(new SlamPoint(0, Vector3.zero, Color.black));
            var e1 = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.zero, Vector3.zero, Color.black));
            mockedRenderer.Verify(r => r.OnItemsAdded(container, e1), Times.Once());
            
            container.AddPointToTrack(new SlamPoint(0, Vector3.one, Color.red));
            var e2 = new AddedEventArgs<SimpleLine>(new SimpleLine(1, Vector3.zero, Vector3.one, Color.black, Color.red));
            mockedRenderer.Verify(r => r.OnItemsAdded(container, e2), Times.Once());
        }

        [Test]
        public void TrackedObjectsContainerSingle()
        {
            var container = new TrackedObjectsContainer();
            var mockedRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            container.SetRenderer(mockedRenderer.Object);
            Assert.AreEqual(0, container.Count);
            
            container.Add(new SlamTrackedObject(0, Vector3.zero, Quaternion.identity, Color.black));
            Assert.AreEqual(1, container.Count);
            var e1 = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.zero, Vector3.zero, Color.black));
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e1), Times.Once());
            
            container.Update(new SlamTrackedObject(0, Vector3.one, Quaternion.identity, Color.red));
            Assert.AreEqual(1, container.Count);
            var e2 = new AddedEventArgs<SimpleLine>(new SimpleLine(1, Vector3.zero, Vector3.one, Color.black, Color.red));
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e2), Times.Once());

            container.Remove(new[] { 0 });
            Assert.AreEqual(0, container.Count);
            var e3 = new RemovedEventArgs(new List<int> { 0, 1 });
            mockedRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), e3), Times.Once());
        }

        [Test]
        public void TrackedObjectsContainerRange()
        {
            var container = new TrackedObjectsContainer();
            var mockedRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            container.SetRenderer(mockedRenderer.Object);
            Assert.AreEqual(0, container.Count);
            
            container.AddRange(new []
            {
                new SlamTrackedObject(0, Vector3.zero, Quaternion.identity, Color.black),
                new SlamTrackedObject(1, Vector3.one, Quaternion.identity, Color.black),
                new SlamTrackedObject(2, Vector3.forward, Quaternion.identity, Color.black)
            });
            Assert.AreEqual(3, container.Count);
            var e1 = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.zero, Vector3.zero, Color.black));
            var e2 = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.one, Vector3.one, Color.black));
            var e3 = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.forward, Vector3.forward, Color.black));
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e1), Times.Once());
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e2), Times.Once());
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e3), Times.Once());
            
            container.Update(new []
            {
                new SlamTrackedObject(0, Vector3.one, Quaternion.identity, Color.red),
                new SlamTrackedObject(1, Vector3.zero, Quaternion.identity, Color.red)
            });
            Assert.AreEqual(3, container.Count);
            var e4 = new AddedEventArgs<SimpleLine>(new SimpleLine(1, Vector3.zero, Vector3.one, Color.black, Color.red));
            var e5 = new AddedEventArgs<SimpleLine>(new SimpleLine(1, Vector3.one, Vector3.zero, Color.black, Color.red));
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e4), Times.Once());
            mockedRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), e5), Times.Once());

            container.Remove(new[] { 0, 1 });
            Assert.AreEqual(1, container.Count);
            var e6 = new RemovedEventArgs(new List<int> { 0, 1 });
            mockedRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), e6), Times.Exactly(2));
        }
    }
}