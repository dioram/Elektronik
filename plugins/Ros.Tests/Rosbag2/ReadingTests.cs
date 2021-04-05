using System.Linq;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros2.Bag.Containers;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using Elektronik.Settings;
using NUnit.Framework;

namespace Elektronik.Ros.Tests.Rosbag2
{
    public class ReadingTests
    {
        private readonly Rosbag2ContainerTree _tree = new("");

        [SetUp]
        public void Setup()
        {
            _tree.Init(new FileScaleSettingsBag() {FilePath = @"test_db.db3"});
        }

        [Test]
        public void ReadMetadata()
        {
            Assert.AreEqual(402, _tree.DBModel?.Table<Message>().Count());
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