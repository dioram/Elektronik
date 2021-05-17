using System.IO;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using NUnit.Framework;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;

namespace Elektronik.Ros.Tests.Rosbag2
{
    public class Parsers
    {
        [Test, Explicit]
        public void ImageParsing()
        {
            using var dbModel = new SQLiteConnection(@"D://calibrator/calibrator_0.db3");
            var messages = dbModel.Table<Message>().OrderBy(m => m.ID).Where(m => m.TopicID == 4);
            using var f = File.CreateText(@"D://calibrator/IMU.csv");
            f.WriteLine("timestamp,linear_acceleration.x,linear_acceleration.y,linear_acceleration.z," +
                        "angular_velocity.x,angular_velocity.y,angular_velocity.z," +
                        "orientation.x,orientation.y,orientation.z,orientation.w");
            foreach (var message in messages)
            {
                var data = (MessageParser.Parse(message.Data, "sensor_msgs/msg/Imu", true)) as Imu;
                Assert.NotNull(data);
#pragma warning disable 8602
                TestContext.WriteLine($"Timestamp: {data.header.stamp.secs}{data.header.stamp.nsecs}");
                f.WriteLine($"{data.header.stamp.secs}{data.header.stamp.nsecs}," +
                            $"{data.linear_acceleration.x},{data.linear_acceleration.y},{data.linear_acceleration.z}," +
                            $"{data.angular_velocity.x},{data.angular_velocity.y},{data.angular_velocity.z}," +
                            $"{data.orientation.x},{data.orientation.y},{data.orientation.z},{data.orientation.w}");
#pragma warning restore 8602
            }
        }

        [Test, Explicit]
        public void Parsing()
        {
            using var dbModel = new SQLiteConnection(@"D://calibrator3_0.db3");
            var messages = dbModel.Table<Message>().OrderBy(m => m.ID).Where(m => m.TopicID == 2);
            using var f = File.CreateText(@"D://IMU.csv");
            f.WriteLine("timestamp.secs,linear_acceleration.x,linear_acceleration.y,linear_acceleration.z," +
                        "angular_velocity.x,angular_velocity.y,angular_velocity.z," +
                        "orientation.x,orientation.y,orientation.z,orientation.w,");
            foreach (var message in messages)
            {
                var data = (MessageParser.Parse(message.Data, "sensor_msgs/msg/Imu", true)) as Imu;
                Assert.NotNull(data);
#pragma warning disable 8602
                TestContext.WriteLine($"Timestamp: {data.header.stamp.secs}{data.header.stamp.nsecs:000000000}");
                f.WriteLine($"{data.header.stamp.secs}{data.header.stamp.nsecs:000000000}," +
                            $"{data.linear_acceleration.x},{data.linear_acceleration.y},{data.linear_acceleration.z}," +
                            $"{data.angular_velocity.x},{data.angular_velocity.y},{data.angular_velocity.z}," +
                            $"{data.orientation.x},{data.orientation.y},{data.orientation.z},{data.orientation.w}");
#pragma warning restore 8602
            }
        }

        [Test, Explicit]
        public void ParsingPose()
        {
            using var dbModel = new SQLiteConnection(@"D://calibrator3_0.db3");
            var messages = dbModel.Table<Message>().OrderBy(m => m.ID).Where(m => m.TopicID == 5);
            using var f = File.CreateText(@"D://pose.csv");
            f.WriteLine("timestamp.secs,pose.x,pose.y,pose.z,rot.x,rot.y,rot.x,rot.w");
            foreach (var message in messages)
            {
                var data = (MessageParser.Parse(message.Data, "geometry_msgs/PoseStamped", true)) as PoseStamped;
                Assert.NotNull(data);
#pragma warning disable 8602
                TestContext.WriteLine($"Timestamp: {data.header.stamp.secs}{data.header.stamp.nsecs:000000000}");
                f.WriteLine($"{data.header.stamp.secs}{data.header.stamp.nsecs:000000000}," +
                            $"{data.pose.position.x},{data.pose.position.y},{data.pose.position.z}," +
                            $"{data.pose.orientation.x},{data.pose.orientation.y}," +
                            $"{data.pose.orientation.z},{data.pose.orientation.w}");
#pragma warning restore 8602
            }
        }

        [Test, Explicit]
        public void ParsingOdometry()
        {
            using var dbModel = new SQLiteConnection(@"D://calibrator3_0.db3");
            var messages = dbModel.Table<Message>().OrderBy(m => m.ID).Where(m => m.TopicID == 1);
            using var f = File.CreateText(@"D://tracker.csv");
            f.WriteLine("timestamp.secs,pose.x,pose.y,pose.z,rot.x,rot.y,rot.x,rot.w");
            foreach (var message in messages)
            {
                var data = (MessageParser.Parse(message.Data, "nav_msgs/Odometry", true)) as Odometry;
                Assert.NotNull(data);
#pragma warning disable 8602
                TestContext.WriteLine($"Timestamp: {data.header.stamp.secs}{data.header.stamp.nsecs:000000000}");
                f.WriteLine($"{data.header.stamp.secs}{data.header.stamp.nsecs:000000000}," +
                            $"{data.pose.pose.position.x},{data.pose.pose.position.y},{data.pose.pose.position.z}," +
                            $"{data.pose.pose.orientation.x},{data.pose.pose.orientation.y}," +
                            $"{data.pose.pose.orientation.z},{data.pose.pose.orientation.w}");
#pragma warning restore 8602
            }
        }

        [Test, Explicit]
        public void ParsingTransform()
        {
            using var dbModel = new SQLiteConnection(@"D://calibrator1_0.db3");
            var messages = dbModel.Table<Message>().OrderBy(m => m.ID).Where(m => m.TopicID == 3);
            using var f = File.CreateText(@"D://transform1.csv");
            f.WriteLine("timestamp.secs,pose.x,pose.y,pose.z,rot.x,rot.y,rot.x,rot.w");
            foreach (var message in messages)
            {
                var data = (MessageParser.Parse(message.Data, "geometry_msgs/TransformStamped", true)) as TransformStamped;
                Assert.NotNull(data);
#pragma warning disable 8602
                TestContext.WriteLine($"Timestamp: {data.header.stamp.secs}{data.header.stamp.nsecs:000000000}");
                f.WriteLine($"{data.header.stamp.secs}{data.header.stamp.nsecs:000000000}," +
                            $"{data.transform.translation.x},{data.transform.translation.y},{data.transform.translation.z}," +
                            $"{data.transform.rotation.x},{data.transform.rotation.y}," +
                            $"{data.transform.rotation.z},{data.transform.rotation.w}");
#pragma warning restore 8602
            }
        }
    }
}