using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Presenters;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Protobuf.Tests.Internal
{
    public class ContainerTreeTests
    {
        private ProtobufContainerTree _tree;
        private const int ChildrenCount = 8;
        private Mock<ISourceTreeNode> _image;

        [SetUp]
        public void Setup()
        {
            _image = new Mock<ISourceTreeNode>();
            _image.SetupGet(ld => ld.DisplayName).Returns("Camera");

            _tree = new ProtobufContainerTree("Protobuf", _image.Object, new SlamDataInfoPresenter("Special info"));

            var p1 = new SlamPoint(1, Vector3.zero, Color.black);
            var p2 = new SlamPoint(2, Vector3.zero, Color.black);
            _tree.Points.Add(p1);
            _tree.Points.Add(p2);
            _tree.Points.AddConnections(new[] { (p1.Id, p2.Id) });
        }

        [Test]
        public void Construction()
        {
            Assert.AreEqual("Protobuf", _tree.DisplayName);
            Assert.AreEqual(ChildrenCount, _tree.Children.Count());
            Assert.AreEqual("Tracked objects", ((ISourceTreeNode)_tree.TrackedObjs).DisplayName);
            Assert.AreEqual("Observations", ((ISourceTreeNode)_tree.Observations).DisplayName);
            Assert.AreEqual("Points", ((ISourceTreeNode)_tree.Points).DisplayName);
            Assert.AreEqual("Lines", ((ISourceTreeNode)_tree.Lines).DisplayName);
            Assert.AreEqual("Planes", ((ISourceTreeNode)_tree.Planes).DisplayName);
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
            var snapshot = _tree.TakeSnapshot() as VirtualSource;
            
            Assert.NotNull(snapshot);
            Assert.AreEqual(ChildrenCount - 3, snapshot.Children.Count(ch => ch is not null));
            var points = snapshot.Children.OfType<IConnectableObjectsContainer<SlamPoint>>().First();
            Assert.AreEqual(2, points.Count);
            Assert.AreEqual(2, _tree.Points.Count);
            Assert.AreEqual(1, points.Connections.Count());
            Assert.AreEqual(1, _tree.Points.Connections.Count());
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
    }
}