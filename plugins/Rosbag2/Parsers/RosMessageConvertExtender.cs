using System;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using RosMessage = RosSharp.RosBridgeClient.Message;

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

            var xOffset = cloud.fields.First(f => f.name.StartsWith("x")).offset;
            var yOffset = cloud.fields.First(f => f.name.StartsWith("y")).offset;
            var zOffset = cloud.fields.First(f => f.name.StartsWith("z")).offset;
            var intensityOffset = cloud.fields.FirstOrDefault(f => f.name.StartsWith("intensity"))?.offset ??
                    cloud.point_step + 1;
            var rgbOffset = cloud.fields.FirstOrDefault(f => f.name.StartsWith("rgb"))?.offset ?? cloud.point_step + 1;
            var rgbaOffset = cloud.fields.FirstOrDefault(f => f.name.StartsWith("rgba"))?.offset ??
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

        public static (Vector3, Quaternion) ToUnity(this Pose pose)
        {
            Vector3 v = new Vector3((float) pose.position.x, (float) pose.position.y, (float) pose.position.z);
            Quaternion q = new Quaternion((float) pose.orientation.x, (float) pose.orientation.y,
                                          (float) pose.orientation.z,
                                          (float) pose.orientation.w);
            Converter?.Convert(ref v, ref q);
            return (v, q);
        }
    }
}