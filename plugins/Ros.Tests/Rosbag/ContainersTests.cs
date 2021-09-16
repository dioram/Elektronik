using System.Linq;
using Elektronik.Containers;
using Elektronik.RosPlugin.Common.Containers;
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
            _tree = new RosbagContainerTree(new RosbagSettings {PathToBag = @"bag_filtered.bag"}, "TMP");
        }

        [TearDown]
        public void TearDown()
        {
            _tree.Dispose();
        }

        [Test]
        public void ActualTopicsTest()
        {
            Assert.AreEqual(6, _tree.ActualTopics.Count);
            Assert.AreEqual(("/rr_robot/mobile_base_controller/odom", "nav_msgs/Odometry"), _tree.ActualTopics[0]);
            Assert.AreEqual(("/tf_static", "tf2_msgs/TFMessage"), _tree.ActualTopics[1]);
        }

        [Test]
        public void TreeTest()
        {
            var children = _tree.Children.ToList();
            Assert.AreEqual(5, children.Count);
            Assert.IsInstanceOf<VirtualSource>(children[0]);
            Assert.AreEqual("rr_robot", children[0].DisplayName);
            Assert.IsInstanceOf<UnknownTypePresenter>(children[1]);
            Assert.AreEqual("tf_static", children[1].DisplayName);
        }
    }
}