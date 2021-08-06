using System.Linq;
using Elektronik.Containers;
using Elektronik.RosPlugin.Ros2.Bag;
using Elektronik.RosPlugin.Ros2.Bag.Containers;
using NUnit.Framework;

namespace Elektronik.Ros.Tests.Rosbag2
{
    public class ReadingTestsYaml
    {
        private readonly Rosbag2ContainerTree _tree = new("");

        [SetUp]
        public void Setup()
        {
            _tree.Init(new Rosbag2Settings() {FilePath = @"metadata.yaml"});
        }

        [Test]
        public void ActualTimestamps()
        {
            var sum = 0;
            foreach (var timestamps in _tree.Timestamps.Values)
            {
                sum += timestamps.Count;
            }
            Assert.AreEqual(282, sum);
        }

        [Test]
        public void ContainersTree()
        {
            Assert.AreEqual(20, _tree.ActualTopics.Count);
            Assert.AreEqual(16, _tree.Children.Count());
            var children = _tree.Children.ToList();
            Assert.IsInstanceOf<VirtualContainer>(children[1]);
            Assert.AreEqual("control", children[1].DisplayName);
            Assert.AreEqual(2, children[1].Children.Count());
            Assert.IsInstanceOf<VisualisationMarkersDBContainer>(children[2]);
            Assert.AreEqual("visualization_planning", children[2].DisplayName);
        }
    }
}