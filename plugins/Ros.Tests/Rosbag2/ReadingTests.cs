using System.Linq;
using Elektronik.Ros.Containers;
using Elektronik.Ros.Rosbag2.Containers;
using Elektronik.Ros.Rosbag2.Data;
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
            // Assert.AreEqual(new[] {"test_db.db3"}, _tree.Metadata.Paths);
            // Assert.AreEqual(1614606828653705041, _tree.Metadata.StartingTime);
        }

        [Test]
        public void ContainersTree()
        {
            Assert.AreEqual(18, _tree.ActualTopics?.Length);
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