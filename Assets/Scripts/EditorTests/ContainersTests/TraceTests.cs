using System.Collections.Generic;
using System.Threading;
using Elektronik.Clouds;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests.ContainersTests
{
    public class TraceTests
    {
        private CloudContainer<SlamPoint> _container;
        private Mock<ICloudRenderer<SimpleLine>> _mockedLineRenderer;

        [SetUp]
        public void Setup()
        {
            _container = new CloudContainer<SlamPoint>();
            _container.AddRange(new[]
            {
                new SlamPoint(0, Vector3.zero, Color.black),
                new SlamPoint(1, Vector3.one, Color.blue),
                new SlamPoint(2, Vector3.right, Color.gray),
            });
            _mockedLineRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            _container.AddRenderer(_mockedLineRenderer.Object);
            _container.TraceEnabled = true;
            _container.TraceDuration = 1000;
        }

        [Test]
        public void UpdateSingle()
        {
            Assert.IsTrue(_container.TraceEnabled);
            Assert.AreEqual(1000, _container.TraceDuration);

            _container.Update(new SlamPoint(0, Vector3.one, Color.blue));
            var addedEventArgs = new AddedEventArgs<SimpleLine>(new SimpleLine(0, Vector3.zero, Vector3.one,
                                                                               Color.black, Color.blue));
            var removedEventArgs = new RemovedEventArgs<SimpleLine>(new List<SimpleLine> { new SimpleLine {Id = 0}, });
            _mockedLineRenderer.Verify(lr => lr.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), addedEventArgs),
                                       Times.Once());
            Thread.Sleep((int)(_container.TraceDuration * 1.1));
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs),
                                       Times.Once());
        }

        [Test]
        public void UpdateRange()
        {
            Assert.IsTrue(_container.TraceEnabled);
            Assert.AreEqual(1000, _container.TraceDuration);

            _container.Update(new[]
            {
                new SlamPoint(0, Vector3.one, Color.blue),
                new SlamPoint(1, Vector3.forward, Color.red)
            });
            var addedEventArgs = new AddedEventArgs<SimpleLine>(new[]
            {
                new SimpleLine(0, Vector3.zero, Vector3.one, Color.black, Color.blue),
                new SimpleLine(1, Vector3.one, Vector3.forward, Color.blue, Color.red),
            });
            var removedEventArgs = new RemovedEventArgs<SimpleLine>(new List<SimpleLine>
            {
                new SimpleLine {Id = 0},
                new SimpleLine {Id = 1},
            });
            _mockedLineRenderer.Verify(lr => lr.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), addedEventArgs),
                                       Times.Once());
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs),
                                       Times.Never());
            Thread.Sleep((int)(_container.TraceDuration * 1.1));
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs),
                                       Times.Once());
        }

        [Test]
        public void UpdateQueue()
        {
            Assert.IsTrue(_container.TraceEnabled);
            Assert.AreEqual(1000, _container.TraceDuration);

            _container.Update(new SlamPoint(0, Vector3.one, Color.blue));
            var addedEventArgs1 = new AddedEventArgs<SimpleLine>(
                new SimpleLine(0, Vector3.zero, Vector3.one, Color.black, Color.blue));
            var addedEventArgs2 = new AddedEventArgs<SimpleLine>(
                new SimpleLine(1, Vector3.one, Vector3.forward, Color.blue, Color.red));
            var removedEventArgs1 = new RemovedEventArgs<SimpleLine>(new List<SimpleLine>
            {
                new SimpleLine {Id = 0},
            });
            var removedEventArgs2 = new RemovedEventArgs<SimpleLine>(new List<SimpleLine>
            {
                new SimpleLine {Id = 1},
            });

            _mockedLineRenderer.Verify(lr => lr.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), addedEventArgs1),
                                       Times.Once());
            Thread.Sleep(_container.TraceDuration / 2);
            _container.Update(new SlamPoint(0, Vector3.forward, Color.red));
            _mockedLineRenderer.Verify(lr => lr.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), addedEventArgs2),
                                       Times.Once());
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs1),
                                       Times.Never());
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs2),
                                       Times.Never());

            Thread.Sleep((int)(_container.TraceDuration * 0.6));
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs1),
                                       Times.Once());
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs2),
                                       Times.Never());
            Thread.Sleep(_container.TraceDuration);
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs2),
                                       Times.Once());
        }

        [Test]
        public void Remove()
        {
            Assert.IsTrue(_container.TraceEnabled);
            Assert.AreEqual(1000, _container.TraceDuration);

            _container.Update(new SlamPoint(0, Vector3.one, Color.blue));
            Assert.AreEqual(3, _container.Count);
            _container.Remove(new SlamPoint(0, Vector3.one, Color.blue));
            Assert.AreEqual(2, _container.Count);

            var addedEventArgs1 = new AddedEventArgs<SimpleLine>(
                new SimpleLine(0, Vector3.zero, Vector3.one, Color.black, Color.blue));
            var removedEventArgs1 = new RemovedEventArgs<SimpleLine>(new List<SimpleLine>
            {
                new SimpleLine {Id = 0},
            });

            _mockedLineRenderer.Verify(lr => lr.OnItemsAdded(It.IsAny<IContainer<SimpleLine>>(), addedEventArgs1),
                                       Times.Once());
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs1),
                                       Times.Never());

            Thread.Sleep((int)(_container.TraceDuration * 1.1));
            _mockedLineRenderer.Verify(lr => lr.OnItemsRemoved(It.IsAny<IContainer<SimpleLine>>(), removedEventArgs1),
                                       Times.Once());
        }
    }
}