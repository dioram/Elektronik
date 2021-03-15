using System;
using System.IO;
using System.Text;
using Elektronik.Rosbag2.Data;
using Elektronik.Rosbag2.RosMessages;
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
            case "visualization_msgs/msg/MarkerArray":
                return ParseMarkerArray(memoryStream);
            default:
                return null;
            }
        }
        
        private static PointCloud2 ParsePointCloud2(Stream data)
        {
            var header = ParseHeader(data);
            var height = ParseUInt32(data);
            var width = ParseUInt32(data);
            var fields = ParseArray(data, ParsePointField);
            var isBigEndian = ParseBool(data);
            var pointStep = ParseUInt32(data);
            var rowStep = ParseUInt32(data);
            var pointData = ParseArray(data, stream => (byte)stream.ReadByte());
            var isDense = ParseBool(data);
            return new PointCloud2(header, height, width, fields, isBigEndian, pointStep, rowStep, pointData, isDense);
        }

        private static MarkerArray ParseMarkerArray(Stream data)
        {
            ParseUInt32(data);
            return new MarkerArray { Markers = ParseArray(data, ParseMarker)};
        }

        private static Marker ParseMarker(Stream data)
        {
            return new Marker
            {
                Header = ParseHeader(data, true),
                Ns = ParseString(data),
                Id = ParseInt32(data),
                Form = (Marker.MarkerForm) ParseUInt32(data),
                Action = (Marker.MarkerAction) ParseUInt32(data),
                Pose = ParsePose(data),
                Scale = ParseVector3(data),
                Color = ParseColor(data),
                Lifetime = ParseTime(data),
                FrameLocked = ParseBool(data),
                Points = ParseArray(data, ParsePoint),
                Colors = ParseArray(data, ParseColor),
                Text = ParseString(data),
                MeshResource = ParseString(data),
                MeshUseEmbeddedMaterials = ParseBool(data),
            };
        }

        private static T[] ParseArray<T>(Stream data, Func<Stream, T> parser)
        {
            var amount = ParseInt32(data);
            var objs = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                objs[i] = parser(data);
            }

            return objs;
        }

        private static T[] ParseArray<T>(Stream data, int count, Func<Stream, T> parser)
        {
            var objs = new T[count];
            for (int i = 0; i < count; i++)
            {
                objs[i] = parser(data);
            }

            return objs;
        }

        private static ColorRGBA ParseColor(Stream data)
        {
            return new ColorRGBA(ParseFloat32(data), ParseFloat32(data), ParseFloat32(data), ParseFloat32(data));
        }

        private static PointField ParsePointField(Stream data)
        {
            return new PointField(ParseString(data), ParseUInt32(data), (byte)data.ReadByte(), ParseUInt32(data));
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
            return new Imu(ParseHeader(data), ParseQuaternion(data), ParseArray(data, 9, ParseFloat64),
                           ParseVector3(data), ParseArray(data, 9, ParseFloat64), ParseVector3(data),
                           ParseArray(data, 9, ParseFloat64));
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
            return new PoseWithCovariance(ParsePose(data), ParseArray(data, 36, ParseFloat64));
        }

        private static Pose ParsePose(Stream data)
        {
            return new Pose(ParsePoint(data), ParseQuaternion(data));
        }

        private static TwistWithCovariance ParseTwistWithCovariance(Stream data)
        {
            return new TwistWithCovariance(ParseTwist(data), ParseArray(data, 36, ParseFloat64));
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

        private static string ParseString(Stream data)
        {
            var len = ParseInt32(data);
            var str = new byte[len];
            data.Read(str, 0, len);
            return Encoding.UTF8.GetString(str).TrimEnd('\0');
        }

        private static int ParseInt32(Stream data)
        {
            Align(data, 4);
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private static UInt32 ParseUInt32(Stream data)
        {
            Align(data, 4);
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static float ParseFloat32(Stream data)
        {
            Align(data, 4);
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        private static double ParseFloat64(Stream data)
        {
            Align(data, 8);
            var buffer = new byte[8];
            data.Read(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        private static bool ParseBool(Stream data)
        {
            return data.ReadByte() != 0;
        }

        private static void Align(Stream data, int len)
        {
            if (len != 8)
            {
                while ((data.Position % len) != 0 && data.Position < data.Length)
                {
                    data.ReadByte();
                }
            }
            else
            {
                while ((data.Position % len) != 4 && data.Position < data.Length)
                {
                    data.ReadByte();
                }
            }
        }
    }
}