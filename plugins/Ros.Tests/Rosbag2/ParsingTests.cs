using System.Linq;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using NUnit.Framework;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Elektronik.Ros.Tests.Rosbag2
{
    public class ParsingTests
    {
#pragma warning disable 8618
        private SQLiteConnection _dbModel;
#pragma warning restore 8618

        [SetUp]
        public void Setup()
        {
            _dbModel = new SQLiteConnection("test_db.db3");
        }

        [Test]
        public void PoseStamped()
        {
            var topic = _dbModel.Table<Topic>().First(t => t.Id == 36);
            var message = _dbModel.Table<Message>().OrderBy(m => m.ID).First(m => m.TopicID == 36);

            var data = (MessageParser.Parse(message.Data, topic.Type, true) as PoseStamped)!;
            var (pos, rot) = data.GetPose()!.ToUnity();
            Assert.AreEqual(new Vector3(18.1172962f, -232.600952f, -0.094754152f), pos);
            Assert.AreEqual(new Quaternion(-0.0218413882f, -0.0231395327f, 0.93242377f, 0.359963149f), rot);
        }

        [Test]
        public void Odometry()
        {
            var topic = _dbModel.Table<Topic>().First(t => t.Id == 60);
            var message = _dbModel.Table<Message>().OrderBy(m => m.ID).First(m => m.TopicID == 60);

            var data = (MessageParser.Parse(message.Data, topic.Type, true) as Odometry)!;
            var (pos, rot) = data.GetPose()!.ToUnity();
            Assert.AreEqual(new Vector3(13.2799072f, -230.354233f, 0f), pos);
            Assert.AreEqual(new Quaternion(0f, 0f, -0.998499155f, 0.0547673702f), rot);
        }

        [Test]
        public void PointCloud2()
        {
            var topic = _dbModel.Table<Topic>().First(t => t.Id == 35);
            var message = _dbModel.Table<Message>().OrderBy(m => m.ID).First(m => m.TopicID == 35);

            var data = (MessageParser.Parse(message.Data, topic.Type, true) as PointCloud2)!;
            Assert.AreEqual(256, data.header.seq);
            Assert.AreEqual(1614606828, data.header.stamp.secs);
            Assert.AreEqual(641145056, data.header.stamp.nsecs);
            Assert.AreEqual("base_link", data.header.frame_id);
            Assert.AreEqual(1, data.height);
            Assert.AreEqual(13524, data.width);
            Assert.AreEqual(4, data.fields.Length);
            Assert.AreEqual(0, data.fields[0].offset);
            Assert.AreEqual(4, data.fields[1].offset);
            Assert.AreEqual(8, data.fields[2].offset);
            Assert.AreEqual(16, data.fields[3].offset);
            Assert.AreEqual(false, data.is_bigendian);
            Assert.AreEqual(32, data.point_step);
            Assert.AreEqual(432768, data.row_step);
            Assert.AreEqual(true, data.is_dense);
            
            var cloud = data.ToSlamPoints();
            Assert.AreEqual(13524, cloud.Length);
            Assert.AreEqual(0, cloud[0].Id);
            Assert.AreEqual(new Vector3(4.41640425f, -44.7025146f, -2.13568068f), cloud[0].Position);
            Assert.AreEqual(Color.red, cloud[0].Color);
            Assert.AreEqual(1, cloud[1].Id);
            Assert.AreEqual(new Vector3(4.58797216f, -44.9095612f, -2.15349817f), cloud[1].Position);
            Assert.AreEqual(Color.red, cloud[1].Color);
        }

        [Test]
        public void VisualisationArraySimple()
        {
            var topic = _dbModel.Table<Topic>().First(t => t.Id == 29);
            var message = _dbModel.Table<Message>().OrderBy(m => m.ID).First(m => m.TopicID == 29);

            var data = (MessageParser.Parse(message.Data, topic.Type, true) as MarkerArray)!;
            Assert.AreEqual(4, data.Markers.Length);
            var markers = data.Markers.Where(m => m.IsSimple).SelectMany(m => m.GetPoints()).ToArray();
            Assert.AreEqual(2, markers.Length);
            Assert.AreEqual(new Vector3(38.8304901f, -180.821518f, 2.57357359f), markers[0].Position);
            Assert.AreEqual(new Color(1f, 1f, 0.00999999978f, 0.409554332f), markers[0].Color);
            Assert.AreEqual(new Vector3(46.5126038f, -250.165802f, 1.51768017f), markers[1].Position);
            Assert.AreEqual(new Color(0.5f, 0.00999999978f, 0.00999999978f, 0.412053257f), markers[1].Color);
        }
    }
}