using System;
using System.IO;
using System.Text;
using Elektronik.Rosbag2.Data;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using RosMessage = RosSharp.RosBridgeClient.Message;
using Path = RosSharp.RosBridgeClient.MessageTypes.Nav.Path;
using UInt32 = System.UInt32;

namespace Elektronik.Rosbag2.Parsers
{
    public static class MessageParser
    {
        public static RosMessage Parse(Message data, Topic topic)
        {
            MemoryStream memoryStream = new MemoryStream(data.Data);
            memoryStream.Position = 0;
            switch (topic.Type)
            {
            case "geometry_msgs/msg/PoseStamped":
                return ParsePoseStamped(memoryStream);
            case "nav_msgs/msg/Odometry":
                return ParseOdometry(memoryStream);
            case "sensor_msgs/msg/Imu":
                return ParseImu(memoryStream);
            case "nav_msgs/msg/Path":
                return ParsePath(memoryStream);
            case "sensor_msgs/msg/PointCloud2":
                return ParsePointCloud2(memoryStream);
            default:
                return null;
            }
        }

        private static PointCloud2 ParsePointCloud2(Stream data)
        {
            var header = ParseHeader(data);
            var height = ParseUInt32(data);
            var width = ParseUInt32(data);
            var fields = ParsePointFieldArray(data);
            var isBigEndian = ParseUInt32(data) != 0;
            var pointStep = ParseUInt32(data);
            var rowStep = ParseUInt32(data);
            var pointData = ParseByteArray(data);
            var isDense = ParseUInt32(data) != 0;
            return new PointCloud2(header, height, width, fields, isBigEndian, pointStep, rowStep, pointData, isDense);
        }

        private static PointField[] ParsePointFieldArray(Stream data)
        {
            var amount = (int) ParseUInt32(data);
            var fields = new PointField[amount];
            for (int i = 0; i < amount; i++)
            {
                fields[i] = ParsePointField(data);
            }

            return fields;
        }

        private static PointField ParsePointField(Stream data)
        {
            return new PointField(ParseString(data), ParseUInt32(data), (byte) ParseUInt32(data), ParseUInt32(data));
        }

        private static Path ParsePath(Stream data)
        {
            var header = ParseHeader(data);
            var amount = (int) ParseUInt32(data);
            var poses = new PoseStamped[amount];
            poses[0] = ParsePoseStamped(data, true, true);
            for (int i = 1; i < amount; i++)
            {
                poses[i] = ParsePoseStamped(data, true);
            }

            return new Path(header, poses);
        }

        private static Imu ParseImu(Stream data)
        {
            return new Imu(ParseHeader(data), ParseQuaternion(data), ParseFloat64Array(data, 9),
                           ParseVector3(data), ParseFloat64Array(data, 9), ParseVector3(data),
                           ParseFloat64Array(data, 9));
        }

        private static Odometry ParseOdometry(Stream data)
        {
            return new Odometry(ParseHeader(data), ParseString(data), ParsePoseWithCovariance(data),
                                ParseTwistWithCovariance(data));
        }

        private static PoseStamped ParsePoseStamped(Stream data, bool noSeq = false, bool strangeHeader = false)
        {
            return new PoseStamped(ParseHeader(data, noSeq, strangeHeader), ParsePose(data));
        }

        private static Header ParseHeader(Stream data, bool noSeq = false, bool strangeHeader = false)
        {
            switch (noSeq)
            {
            case true when strangeHeader:
            {
                var time = ParseTime(data);
                var str = ParseString(data);
                var i = ParseUInt32(data);
                return new Header(0, time, str);
            }
            case true:
                return new Header(0, ParseTime(data), ParseString(data));
            default:
                return new Header(ParseUInt32(data), ParseTime(data), ParseString(data));
            }
        }

        private static PoseWithCovariance ParsePoseWithCovariance(Stream data)
        {
            return new PoseWithCovariance(ParsePose(data), ParseFloat64Array(data, 36));
        }

        private static Pose ParsePose(Stream data)
        {
            return new Pose(ParsePoint(data), ParseQuaternion(data));
        }

        private static TwistWithCovariance ParseTwistWithCovariance(Stream data)
        {
            return new TwistWithCovariance(ParseTwist(data), ParseFloat64Array(data, 36));
        }

        private static Twist ParseTwist(Stream data)
        {
            return new Twist(ParseVector3(data), ParseVector3(data));
        }

        private static Point ParsePoint(Stream data)
        {
            return new Point(ParseFloat64(data), ParseFloat64(data), ParseFloat64(data));
        }

        private static Vector3 ParseVector3(Stream data)
        {
            return new Vector3(ParseFloat64(data), ParseFloat64(data), ParseFloat64(data));
        }

        private static Quaternion ParseQuaternion(Stream data)
        {
            return new Quaternion(ParseFloat64(data), ParseFloat64(data), ParseFloat64(data), ParseFloat64(data));
        }

        private static Time ParseTime(Stream data)
        {
            return new Time(ParseUInt32(data), ParseUInt32(data));
        }

        private static UInt32 ParseUInt32(Stream data)
        {
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static double ParseFloat64(Stream data)
        {
            var buffer = new byte[8];
            data.Read(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        private static double[] ParseFloat64Array(Stream data, int count)
        {
            var result = new double[count];
            for (int i = 0; i < count; i++)
            {
                var buffer = new byte[8];
                data.Read(buffer, 0, 8);
                result[i] = BitConverter.ToDouble(buffer, 0);
            }

            return result;
        }

        private static byte[] ParseByteArray(Stream data)
        {
            var size = ParseUInt32(data);
            var result = new byte[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = (byte) data.ReadByte();
            }

            return result;
        }

        private static string ParseString(Stream data)
        {
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            var len = FixLen(BitConverter.ToInt32(buffer, 0));
            var str = new byte[len];
            data.Read(str, 0, len);
            return Encoding.UTF8.GetString(str);
        }

        private static int FixLen(int len)
        {
            while (len % 4 != 0)
            {
                len++;
            }

            return len;
        }
    }
}