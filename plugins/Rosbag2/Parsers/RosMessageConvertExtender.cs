using System;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using UnityEngine;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;
using Time = RosSharp.RosBridgeClient.MessageTypes.Std.Time;

namespace Elektronik.Rosbag2.Parsers
{
    public static class RosMessageConvertExtender
    {
        public static ICSConverter Converter;

        public static Pose GetPose(this RosMessage rosMessage)
        {
            var field = rosMessage.GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.PropertyType == typeof(Pose));
            if (field != null) return (Pose) field.GetValue(rosMessage);

            field = rosMessage.GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.PropertyType == typeof(PoseWithCovariance));
            if (field != null) return ((PoseWithCovariance) field.GetValue(rosMessage)).pose;

            field = rosMessage.GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.PropertyType == typeof(PoseWithCovarianceStamped));
            if (field != null) return ((PoseWithCovarianceStamped) field.GetValue(rosMessage)).pose.pose;

            throw new NotSupportedException("Can't convert to SlamTrackedObject message without pose.");
        }

        public static SlamPoint[] ToSlamPoints(this PointCloud2 cloud)
        {
            var res = new SlamPoint[cloud.data.Length / cloud.point_step];

            var xOffset = cloud.fields.First(f => f.name == "x").offset;
            var yOffset = cloud.fields.First(f => f.name == "y").offset;
            var zOffset = cloud.fields.First(f => f.name == "z").offset;
            var intensityOffset = cloud.fields.FirstOrDefault(f => f.name == "intensity")?.offset ??
                    cloud.point_step + 1;
            var rgbOffset = cloud.fields.FirstOrDefault(f => f.name == "rgb")?.offset ?? cloud.point_step + 1;
            var rgbaOffset = cloud.fields.FirstOrDefault(f => f.name == "rgba")?.offset ??
                    cloud.point_step + 1;

            for (int i = 0; i < cloud.data.Length / cloud.point_step; i++)
            {
                var x = BitConverter.ToSingle(cloud.data, (int) (i * cloud.point_step + xOffset));
                var y = BitConverter.ToSingle(cloud.data, (int) (i * cloud.point_step + yOffset));
                var z = BitConverter.ToSingle(cloud.data, (int) (i * cloud.point_step + zOffset));

                Color color = Color.black;
                if (intensityOffset < cloud.point_step)
                {
                    var intensity = BitConverter.ToSingle(cloud.data, (int) (i * cloud.point_step + intensityOffset));
                    color = ColorFromIntensity(intensity);
                }
                else if (rgbOffset < cloud.point_step)
                {
                    byte r = cloud.data[i * cloud.point_step + cloud.point_step + 0];
                    byte g = cloud.data[i * cloud.point_step + cloud.point_step + 1];
                    byte b = cloud.data[i * cloud.point_step + cloud.point_step + 2];
                    color = new Color(r / (float) byte.MaxValue, g / (float) byte.MaxValue, b / (float) byte.MaxValue);
                }
                else if (rgbaOffset < cloud.point_step)
                {
                    byte a = cloud.data[i * cloud.point_step + cloud.point_step + 0];
                    byte r = cloud.data[i * cloud.point_step + cloud.point_step + 1];
                    byte g = cloud.data[i * cloud.point_step + cloud.point_step + 2];
                    byte b = cloud.data[i * cloud.point_step + cloud.point_step + 3];
                    color = new Color(r / (float) byte.MaxValue, g / (float) byte.MaxValue, b / (float) byte.MaxValue,
                                      a / (float) byte.MaxValue);
                }

                var pos = new Vector3(x, y, z);
                Converter?.Convert(ref pos);
                res[i] = new SlamPoint(i, pos, color);
            }

            return res;
        }

        public static long ToLong(this Time time) => (((long) time.secs) << 32) + time.nsecs;

        public static Color ToUnity(this ColorRGBA color) => new Color(color.r, color.g, color.b, color.a);

        private static Color ColorFromIntensity(float intensity) =>
                intensity switch
                {
                    > 1 => Color.red,
                    > 0.75f => Color.yellow,
                    > 0.5f => Color.green,
                    > 0.25f => Color.cyan,
                    > 0f => Color.blue,
                    _ => Color.black,
                };

        private static SlamTrackedObject FromPose(Pose pose)
        {
            var (v, q) = pose.ToUnity();
            return new SlamTrackedObject
            {
                Id = 0,
                Position = v,
                Rotation = q,
            };
        }

        public static (Vector3 pos, Quaternion rot) ToUnity(this Pose pose)
        {
            Vector3 v = new Vector3((float) pose.position.x, (float) pose.position.y, (float) pose.position.z);
            Quaternion q = new Quaternion((float) pose.orientation.x, (float) pose.orientation.y,
                                          (float) pose.orientation.z,
                                          (float) pose.orientation.w);
            Converter?.Convert(ref v, ref q);
            return (v, q);
        }

        public static Vector3 ToUnity(this RosVector3 vector3)
        {
            var res = new Vector3((float) vector3.x, (float) vector3.y, (float) vector3.z);
            Converter?.Convert(ref res);
            return res;
        }
        
        public static Vector3 ToUnity(this Point point)
        {
            var res = new Vector3((float) point.x, (float) point.y, (float) point.z);
            Converter?.Convert(ref res);
            return res;
        }
    }
}