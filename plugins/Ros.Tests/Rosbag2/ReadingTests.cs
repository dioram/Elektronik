using System.Linq;
using Elektronik.Containers;
using Elektronik.RosPlugin.Ros2.Bag;
using Elektronik.RosPlugin.Ros2.Bag.Containers;
using NUnit.Framework;

namespace Elektronik.Ros.Tests.Rosbag2
{
    public class ReadingTests
    {
        [Test, Platform("Win")]
        public void ActualTimestamps()
        {
            var tree = new Rosbag2ContainerTree(new Rosbag2Settings() {PathToFile = @"test_db.db3"});
            var sum = tree.Timestamps.Values.Sum(timestamps => timestamps.Count);
            Assert.AreEqual(141, sum);
        }

        [Test, Platform("Win")]
        public void ContainersTree()
        {
            var tree = new Rosbag2ContainerTree(new Rosbag2Settings() {PathToFile = @"test_db.db3"});
            Assert.AreEqual(20, tree.ActualTopics.Count);
            Assert.AreEqual(16, tree.Children.Count());
            var children = tree.Children.ToList();
            Assert.IsInstanceOf<VirtualSource>(children[1]);
            Assert.AreEqual("control", children[1].DisplayName);
            Assert.AreEqual(2, children[1].Children.Count());
            Assert.IsInstanceOf<VisualisationMarkersDBContainer>(children[2]);
            Assert.AreEqual("visualization_planning", children[2].DisplayName);
        }
    }
}