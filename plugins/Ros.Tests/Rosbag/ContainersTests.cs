using System.Linq;
using Elektronik.Containers;
using Elektronik.RosPlugin.Ros.Bag;
using NUnit.Framework;

namespace Elektronik.Ros.Tests.Rosbag
{
    public class ContainersTests
    {
#pragma warning disable 8618
        private RosbagContainerTree _tree;
#pragma warning restore 8618

        [SetUp]
        public void Setup()
        {
            _tree = new RosbagContainerTree("TMP");
            _tree.Init(new RosbagSettings {FilePath = @"bag_filtered.bag"});
        }

        [TearDown]
        public void TearDown()
        {
            _tree.Reset();
        }

        [Test]
        public void ActualTopicsTest()
        {
            Assert.AreEqual(2, _tree.ActualTopics.Count);
            Assert.AreEqual(("/rr_robot/mobile_base_controller/odom", "nav_msgs/Odometry"), _tree.ActualTopics[0]);
            Assert.AreEqual(("/odometry/filtered", "nav_msgs/Odometry"), _tree.ActualTopics[1]);
        }

        [Test]
        public void TreeTest()
        {
            var children = _tree.Children.ToList();
            Assert.AreEqual(2, children.Count);
            Assert.IsInstanceOf<TrackedObjectsContainer>(children[0]);
            Assert.AreEqual("rr_robot/mobile_base_controller/odom", children[0].DisplayName);
            Assert.IsInstanceOf<TrackedObjectsContainer>(children[1]);
            Assert.AreEqual("odometry/filtered", children[1].DisplayName);
        }
    }
}