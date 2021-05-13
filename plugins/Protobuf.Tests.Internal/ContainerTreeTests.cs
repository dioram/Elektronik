using System.Collections.Generic;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Presenters;
using Elektronik.Renderers;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class ContainerTreeTests
    {
        private ProtobufContainerTree _tree;
        private const int ChildrenCount = 7;
        private Mock<ISourceTree> _image;

        [SetUp]
        public void Setup()
        {
            _image = new Mock<ISourceTree>();
            _image.SetupGet(ld => ld.DisplayName).Returns("Camera");

            _tree = new ProtobufContainerTree("Protobuf",
                                              _image.Object,
                                              new SlamDataInfoPresenter("Special info"));

            var p1 = new SlamPoint(1, Vector3.zero, Color.black);
            var p2 = new SlamPoint(2, Vector3.zero, Color.black);
            _tree.Points.Add(p1);
            _tree.Points.Add(p2);
            _tree.Points.AddConnections(new[] {(p1.Id, p2.Id)});
        }

        [Test]
        public void Construction()
        {
            Assert.AreEqual("Protobuf", _tree.DisplayName);
            Assert.AreEqual(ChildrenCount, _tree.Children.Count());
            Assert.AreEqual("Tracked objects", ((ISourceTree) _tree.TrackedObjs).DisplayName);
            Assert.AreEqual("Observations", ((ISourceTree) _tree.Observations).DisplayName);
            Assert.AreEqual("Points", ((ISourceTree) _tree.Points).DisplayName);
            Assert.AreEqual("Lines", ((ISourceTree) _tree.Lines).DisplayName);
            Assert.AreEqual("Infinite planes", ((ISourceTree) _tree.InfinitePlanes).DisplayName);
            Assert.AreEqual("Camera", _tree.Image.DisplayName);
            Assert.AreEqual("Special info", _tree.SpecialInfo.DisplayName);
            Assert.AreEqual(true, _tree.IsVisible);
            Assert.AreEqual(true, _tree.ShowButton);
        }

        [Test]
        public void Clear()
        {
            _tree.Clear();
            Assert.AreEqual(ChildrenCount, _tree.Children.Count());
            Assert.AreEqual(0, _tree.Points.Connections.Count());
            Assert.AreEqual(0, _tree.Points.Count());
        }

        [Test]
        public void TakeSnapshot()
        {
            var snapshot = _tree.TakeSnapshot() as VirtualContainer;
            Assert.NotNull(snapshot);
            Assert.AreEqual(ChildrenCount - 2, snapshot.Children.Count(ch => ch is not null));
            var points = snapshot.Children.OfType<IConnectableObjectsContainer<SlamPoint>>().First();
            Assert.AreEqual(2, points.Count);
            Assert.AreEqual(2, _tree.Points.Count);
            Assert.AreEqual(1, points.Connections.Count());
            Assert.AreEqual(1, _tree.Points.Connections.Count());
        }

        [Test]
        public void WriteSnapshot()
        {
            var mockedRecorder = new Mock<IDataRecorderPlugin>();
            var container = new ProtobufContainerTree("Test", _image.Object);

            container.WriteSnapshot(mockedRecorder.Object);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), It.IsAny<IList<SlamPoint>>()), Times.Once);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), It.IsAny<IList<SlamObservation>>()), Times.Once);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), It.IsAny<IList<SlamTrackedObject>>()), Times.Once);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), It.IsAny<IList<SlamLine>>()), Times.Once);
            mockedRecorder.Verify(r => r.OnAdded(It.IsAny<string>(), It.IsAny<IList<SlamInfinitePlane>>()), Times.Once);
            mockedRecorder.Verify(
                r => r.OnConnectionsUpdated<SlamPoint>(It.IsAny<string>(), It.IsAny<IList<(int, int)>>()), Times.Once);
            mockedRecorder.Verify(
                r => r.OnConnectionsUpdated<SlamObservation>(It.IsAny<string>(), It.IsAny<IList<(int, int)>>()),
                Times.Once);
        }

        [Test]
        public void Visible()
        {
            _tree.IsVisible = false;
            foreach (var v in _tree.Children.OfType<IVisible>())
            {
                Assert.AreEqual(false, v.IsVisible);
            }

            _tree.IsVisible = true;
            foreach (var v in _tree.Children.OfType<IVisible>())
            {
                Assert.AreEqual(true, v.IsVisible);
            }
        }

        [Test]
        public void Renderer()
        {
            // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
            var renderer = new ImageRenderer();
            _tree.SetRenderer(renderer);

            _image.Verify(i => i.SetRenderer(renderer), Times.Once);
        }
    }
}