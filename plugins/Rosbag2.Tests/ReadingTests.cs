using System.Linq;
using Elektronik.Containers;
using Elektronik.Rosbag2;
using Elektronik.Rosbag2.Data;
using NUnit.Framework;

namespace Rosbag2.Tests
{
    public class ReadingTests
    {
        private readonly Rosbag2ContainerTree _tree = new("");

        [SetUp]
        public void Setup()
        {
            _tree.Init(@"test_db.db3");
        }

        [Test]
        public void ReadMetadata()
        {
            Assert.AreEqual(402, _tree.DBModel.Table<Message>().Count());
            // Assert.AreEqual(new[] {"test_db.db3"}, _tree.Metadata.Paths);
            // Assert.AreEqual(1614606828653705041, _tree.Metadata.StartingTime);
        }

        [Test]
        public void ContainersTree()
        {
            Assert.AreEqual(new[]
                            {
                                new Topic {Id = 6},
                                new Topic {Id = 27},
                                new Topic {Id = 36},
                                new Topic {Id = 38},
                                new Topic {Id = 42},
                                new Topic {Id = 44},
                                new Topic {Id = 48},
                                new Topic {Id = 60},
                            },
                            _tree.ActualTopics);
            Assert.AreEqual(6, _tree.Children.Count());
            var children = _tree.Children.ToList();
            Assert.IsInstanceOf<VirtualContainer>(children[0]);
            Assert.AreEqual("control", children[0].DisplayName);
            Assert.AreEqual(2, children[0].Children.Count());
            Assert.IsInstanceOf<TrackedObjectsContainer>(children[2]);
            Assert.AreEqual("gnss_rviz/pose", children[2].DisplayName);
        }
    }
}