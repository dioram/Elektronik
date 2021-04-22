using System.Linq;
using System.Threading.Tasks;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros.Bag.Parsers;
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
        public async Task MessageDataHeaderTest()
        {
            var messages = await _parser
                    .ReadMessagesAsync()
                    .Select(m => MessageParser.Parse(m.Data, m.TopicType ?? "", false))
                    .Where(m => m is not null)
                    .Select(m => m!)
                    .ToListAsync();
            Assert.AreEqual(24027, messages.Count);
            Assert.IsInstanceOf<Odometry>(messages[0]);
        }

        [Test]
        public async Task MessagesTest()
        {
            var messages = await _parser.ReadMessagesAsync().ToListAsync();
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