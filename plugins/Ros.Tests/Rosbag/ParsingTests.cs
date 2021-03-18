using System.Linq;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
using Elektronik.RosPlugin.Ros.Bag.Parsers.Records;
using NUnit.Framework;
using RosSharp.RosBridgeClient.MessageTypes.Nav;

namespace Elektronik.Ros.Tests.Rosbag
{
    public class ParsingTests
    {
#pragma warning disable 8618
        private BagParser _parser;
#pragma warning restore 8618

        [SetUp]
        public void Setup()
        {
            _parser = new BagParser(@"bag_filtered.bag");
        }

        [TearDown]
        public void TearDown()
        {
            _parser.Dispose();
        }

        [Test]
        public void VersionTest()
        {
            Assert.AreEqual((2, 0), _parser.Version);
        }

        [Test]
        public void BagHeaderTest()
        {
            var bagHeader = _parser.ReadAll().OfType<BagHeader>().Single();
            Assert.AreEqual(0x03, bagHeader.Op);
            Assert.AreEqual(12485221, bagHeader.IndexPos);
            Assert.AreEqual(6, bagHeader.ConnectionsCount);
            Assert.AreEqual(16, bagHeader.ChunkCount);
        }

        [Test]
        public void ReadingRecordsTest()
        {
            var types = _parser.ReadAll().Select(r => r.GetType()).ToList();
            var bagHeader = (BagHeader) _parser.ReadAll().First();
            Assert.AreEqual(1, types.Count(t => t == typeof(BagHeader)));
            Assert.AreEqual(bagHeader.ChunkCount, types.Count(t => t == typeof(Chunk)));
            Assert.AreEqual(bagHeader.ConnectionsCount, types.Count(t => t == typeof(Connection)));
        }

        [Test]
        public void ConnectionsTest()
        {
            var connections = _parser.ReadAll().OfType<Connection>().ToList();
            Assert.AreEqual(6, connections.Count);
            Assert.AreEqual("nav_msgs/Odometry", connections[0].Type);
            Assert.AreEqual("/rr_robot/mobile_base_controller/odom", connections[0].Topic);
            Assert.AreEqual("tf2_msgs/TFMessage", connections[1].Type);
            Assert.AreEqual("/tf_static", connections[1].Topic);
        }

        [Test]
        public void MessageDataHeaderTest()
        {
            var messages = _parser
                    .ReadMessages()
                    .Select(m => MessageParser.Parse(m.Data, m.TopicType ?? "", false))
                    .Where(m => m is not null)
                    .Select(m => m!)
                    .ToList();
            Assert.AreEqual(10068, messages.Count);
            Assert.IsInstanceOf<Odometry>(messages[0]);
        }

        [Test]
        public void MessagesTest()
        {
            var messages = _parser.ReadMessages().ToList();
            Assert.AreEqual(24027, messages.Count);
            Assert.AreEqual(5015, messages.Count(m => m.TopicName == "/odometry/filtered"));
            Assert.AreEqual(5053, messages.Count(m => m.TopicName == "/rr_robot/mobile_base_controller/odom"));
        }

        [Test]
        public void TopicsTest()
        {
            var topics = _parser.GetTopics().Select(t => (t.Topic, t.Type)).ToList();
            Assert.AreEqual(6, topics.Count);
            Assert.AreEqual(("/rr_robot/mobile_base_controller/odom", "nav_msgs/Odometry"), topics[0]);
        }
    }
}