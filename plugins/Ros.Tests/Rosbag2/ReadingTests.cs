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
            Assert.AreEqual(18, _tree.ActualTopics.Count);
            Assert.AreEqual(14, _tree.Children.Count());
            var children = _tree.Children.ToList();
            Assert.IsInstanceOf<VirtualContainer>(children[0]);
            Assert.AreEqual("control", children[0].DisplayName);
            Assert.AreEqual(2, children[0].Children.Count());
            Assert.IsInstanceOf<VisualisationMarkersDBContainer>(children[1]);
            Assert.AreEqual("visualization_planning", children[1].DisplayName);
        }
    }
}