using System;
using System.IO;
using System.Text;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using RosMessage = RosSharp.RosBridgeClient.Message;
using Path = RosSharp.RosBridgeClient.MessageTypes.Nav.Path;
using UInt32 = System.UInt32;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public static class MessageParser
    {
        public static RosMessage? Parse(byte[] data, string topic, bool cdr)
        {
            MemoryStream memoryStream = new(data);
            memoryStream.Position = 0;
            switch (topic)
            {
            case "geometry_msgs/msg/PoseStamped":
            case "geometry_msgs/PoseStamped":
                return ParsePoseStamped(memoryStream, cdr);
            case "nav_msgs/msg/Odometry":
            case "nav_msgs/Odometry":
                return ParseOdometry(memoryStream, cdr);
            case "sensor_msgs/msg/Imu":
            case "sensor_msgs/Imu":
                return ParseImu(memoryStream, cdr);
            case "nav_msgs/msg/Path":
            case "nav_msgs/Path":
                return ParsePath(memoryStream, cdr);
            case "sensor_msgs/msg/PointCloud2":
            case "sensor_msgs/PointCloud2":
                return ParsePointCloud2(memoryStream, cdr);
            case "visualization_msgs/msg/MarkerArray":
            case "visualization_msgs/MarkerArray":
                return ParseMarkerArray(memoryStream, cdr);
            case "sensor_msgs/msg/Image":
            case "sensor_msgs/Image":
                return ParseImage(memoryStream, cdr);
            default:
                return null;
            }
        }

        private static PointCloud2 ParsePointCloud2(Stream data, bool cdr)
        {
            var header = ParseHeader(data, cdr);
            var height = ParseUInt32(data, cdr);
            var width = ParseUInt32(data, cdr);
            var fields = ParseArray(data, ParsePointField, cdr);
            var isBigEndian = ParseBool(data, cdr);
            var pointStep = ParseUInt32(data, cdr);
            var rowStep = ParseUInt32(data, cdr);
            var pointData = ParseByteArray(data, cdr);
            var isDense = ParseBool(data, cdr);
            return new PointCloud2(header, height, width, fields, isBigEndian, pointStep, rowStep, pointData, isDense);
        }

        private static Image ParseImage(Stream data, bool cdr)
        {
            return new Image(ParseHeader(data, cdr), 
                             ParseUInt32(data, cdr), 
                             ParseUInt32(data, cdr),
                             ParseString(data, cdr), 
                             (byte) data.ReadByte(), 
                             ParseUInt32(data, cdr),
                             ParseByteArray(data, cdr));
        }

        private static MarkerArray ParseMarkerArray(Stream data, bool cdr)
        {
            ParseUInt32(data, cdr);
            return new MarkerArray {Markers = ParseArray(data, ParseMarker, cdr)};
        }

        private static Marker ParseMarker(Stream data, bool cdr)
        {
            return new Marker
            {
                Header = ParseHeader(data, cdr, true),
                Ns = ParseString(data, cdr),
                Id = ParseInt32(data, cdr),
                Form = (Marker.MarkerForm) ParseUInt32(data, cdr),
                Action = (Marker.MarkerAction) ParseUInt32(data, cdr),
                Pose = ParsePose(data, cdr),
                Scale = ParseVector3(data, cdr),
                Color = ParseColor(data, cdr),
                Lifetime = ParseTime(data, cdr),
                FrameLocked = ParseBool(data, cdr),
                Points = ParseArray(data, ParsePoint, cdr),
                Colors = ParseArray(data, ParseColor, cdr),
                Text = ParseString(data, cdr),
                MeshResource = ParseString(data, cdr),
                MeshUseEmbeddedMaterials = ParseBool(data, cdr),
            };
        }

        private static byte[] ParseByteArray(Stream data, bool cdr)
        {
            var amount = ParseInt32(data, cdr);
            var bytes = new byte[amount];
            data.Read(bytes, 0, amount);
            return bytes;
        }

        private static T[] ParseArray<T>(Stream data, Func<Stream, bool, T> parser, bool cdr)
        {
            var amount = ParseInt32(data, cdr);
            var objs = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                objs[i] = parser(data, cdr);
            }

            return objs;
        }

        private static T[] ParseArray<T>(Stream data, int count, Func<Stream, bool, T> parser, bool cdr)
        {
            var objs = new T[count];
            for (int i = 0; i < count; i++)
            {
                objs[i] = parser(data, cdr);
            }

            return objs;
        }

        private static ColorRGBA ParseColor(Stream data, bool cdr)
        {
            return new(ParseFloat32(data, cdr), ParseFloat32(data, cdr), ParseFloat32(data, cdr),
                       ParseFloat32(data, cdr));
        }

        private static PointField ParsePointField(Stream data, bool cdr)
        {
            return new PointField(ParseString(data, cdr), ParseUInt32(data, cdr), (byte) data.ReadByte(),
                                  ParseUInt32(data, cdr));
        }

        private static Path ParsePath(Stream data, bool cdr)
        {
            var header = ParseHeader(data, cdr);
            var amount = (int) ParseUInt32(data, cdr);
            var poses = new PoseStamped[amount];
            poses[0] = ParsePoseStamped(data, true, true);
            for (int i = 1; i < amount; i++)
            {
                poses[i] = ParsePoseStamped(data, true);
            }

            return new Path(header, poses);
        }

        private static Imu ParseImu(Stream data, bool cdr)
        {
            return new Imu(ParseHeader(data, cdr), ParseQuaternion(data, cdr), ParseArray(data, 9, ParseFloat64, cdr),
                           ParseVector3(data, cdr), ParseArray(data, 9, ParseFloat64, cdr), ParseVector3(data, cdr),
                           ParseArray(data, 9, ParseFloat64, cdr));
        }

        private static Odometry ParseOdometry(Stream data, bool cdr)
        {
            return new Odometry(ParseHeader(data, cdr), ParseString(data, cdr), ParsePoseWithCovariance(data, cdr),
                                ParseTwistWithCovariance(data, cdr));
        }

        private static PoseStamped ParsePoseStamped(Stream data, bool cdr, bool noSeq = false,
                                                    bool strangeHeader = false)
        {
            return new PoseStamped(ParseHeader(data, cdr, noSeq, strangeHeader), ParsePose(data, cdr));
        }

        private static Header ParseHeader(Stream data, bool cdr, bool noSeq = false, bool strangeHeader = false)
        {
            switch (noSeq)
            {
            case true when strangeHeader:
            {
                var time = ParseTime(data, cdr);
                var str = ParseString(data, cdr);
                var _ = ParseUInt32(data, cdr);
                return new Header(0, time, str);
            }
            case true:
                return new Header(0, ParseTime(data, cdr), ParseString(data, cdr));
            default:
                return new Header(ParseUInt32(data, cdr), ParseTime(data, cdr), ParseString(data, cdr));
            }
        }

        private static PoseWithCovariance ParsePoseWithCovariance(Stream data, bool cdr)
        {
            return new PoseWithCovariance(ParsePose(data, cdr), ParseArray(data, 36, ParseFloat64, cdr));
        }

        private static Pose ParsePose(Stream data, bool cdr)
        {
            return new Pose(ParsePoint(data, cdr), ParseQuaternion(data, cdr));
        }

        private static TwistWithCovariance ParseTwistWithCovariance(Stream data, bool cdr)
        {
            return new TwistWithCovariance(ParseTwist(data, cdr), ParseArray(data, 36, ParseFloat64, cdr));
        }

        private static Twist ParseTwist(Stream data, bool cdr)
        {
            return new Twist(ParseVector3(data, cdr), ParseVector3(data, cdr));
        }

        private static Point ParsePoint(Stream data, bool cdr)
        {
            return new Point(ParseFloat64(data, cdr), ParseFloat64(data, cdr), ParseFloat64(data, cdr));
        }

        private static Vector3 ParseVector3(Stream data, bool cdr)
        {
            return new Vector3(ParseFloat64(data, cdr), ParseFloat64(data, cdr), ParseFloat64(data, cdr));
        }

        private static Quaternion ParseQuaternion(Stream data, bool cdr)
        {
            return new Quaternion(ParseFloat64(data, cdr), ParseFloat64(data, cdr), ParseFloat64(data, cdr),
                                  ParseFloat64(data, cdr));
        }

        private static Time ParseTime(Stream data, bool cdr)
        {
            return new Time(ParseUInt32(data, cdr), ParseUInt32(data, cdr));
        }

        private static string ParseString(Stream data, bool cdr)
        {
            var len = ParseInt32(data, cdr);
            var str = new byte[len];
            data.Read(str, 0, len);
            return Encoding.UTF8.GetString(str).TrimEnd('\0');
        }

        private static int ParseInt32(Stream data, bool cdr)
        {
            if (cdr) Align(data, 4);
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        private static UInt32 ParseUInt32(Stream data, bool cdr)
        {
            if (cdr) Align(data, 4);
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static float ParseFloat32(Stream data, bool cdr)
        {
            if (cdr) Align(data, 4);
            var buffer = new byte[4];
            data.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        private static double ParseFloat64(Stream data, bool cdr)
        {
            if (cdr) Align(data, 8);
            var buffer = new byte[8];
            data.Read(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        // ReSharper disable once UnusedParameter.Local
        private static bool ParseBool(Stream data, bool cdr)
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