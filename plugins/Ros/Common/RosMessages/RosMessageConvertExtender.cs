using System;
using System.Collections;
using System.Linq;
using System.Text;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using UnityEngine;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using RosQuaternion = RosSharp.RosBridgeClient.MessageTypes.Geometry.Quaternion;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;
using Time = RosSharp.RosBridgeClient.MessageTypes.Std.Time;
using Transform = RosSharp.RosBridgeClient.MessageTypes.Geometry.Transform;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public static class RosMessageConvertExtender
    {
        public static ICSConverter? Converter;

        public static string[] GetMessagePropertyNames(Type messageType, string prefix = "")
        {
            if (!messageType.IsSubclassOf(typeof(RosMessage)))
            {
                return new[] {prefix};
            }

            if (messageType.Namespace == "Vector3" || messageType.Name == nameof(Point))
            {
                return new[] {$"{prefix}.x", $"{prefix}.y", $"{prefix}.z"};
            }

            if (messageType.Namespace == "Quaternion")
            {
                return new[] {$"{prefix}.x", $"{prefix}.y", $"{prefix}.z", $"{prefix}.w"};
            }

            if (messageType.Name == nameof(Header)) return new[] {"timestamp, frame_id"};
            
            return messageType
                    .GetProperties()
                    .SelectMany(p => GetMessagePropertyNames(p.PropertyType, $"{prefix}.{p.Name}"))
                    .Select(s => s.TrimStart('.'))
                    .ToArray();
        }

        public static string[] GetData(this RosMessage message)
        {
            return message.GetType()
                    .GetProperties()
                    .Where(p => p.Name != nameof(Header.seq))
                    .SelectMany(p => GetData(p.GetValue(message)))
                    .ToArray();
        }

        public static Pose? GetPose(this RosMessage rosMessage) => rosMessage switch
        {
            Odometry odometry => odometry.pose.pose,
            PoseStamped poseStamped => poseStamped.pose,
            PoseWithCovariance poseWithCovariance => poseWithCovariance.pose,
            PoseWithCovarianceStamped poseWithCovarianceStamped => poseWithCovarianceStamped.pose.pose,
            _ => null,
        };

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
                    byte r = cloud.data[i * cloud.point_step + rgbOffset + 0];
                    byte g = cloud.data[i * cloud.point_step + rgbOffset + 1];
                    byte b = cloud.data[i * cloud.point_step + rgbOffset + 2];
                    color = new Color(r / (float) byte.MaxValue, g / (float) byte.MaxValue, b / (float) byte.MaxValue);
                }
                else if (rgbaOffset < cloud.point_step)
                {
                    byte a = cloud.data[i * cloud.point_step + rgbaOffset + 0];
                    byte r = cloud.data[i * cloud.point_step + rgbaOffset + 1];
                    byte g = cloud.data[i * cloud.point_step + rgbaOffset + 2];
                    byte b = cloud.data[i * cloud.point_step + rgbaOffset + 3];
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

        public static Color ColorFromIntensity(float intensity) =>
                intensity switch
                {
                    > 1 => Color.red,
                    > 0.75f => Color.yellow,
                    > 0.5f => Color.green,
                    > 0.25f => Color.cyan,
                    > 0f => Color.blue,
                    _ => Color.black,
                };

        public static SlamTrackedObject ToTracked(this Pose pose)
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
            var v = new Vector3((float) pose.position.x, (float) pose.position.y, (float) pose.position.z);
            var q = new Quaternion((float) pose.orientation.x, (float) pose.orientation.y,
                                   (float) pose.orientation.z, (float) pose.orientation.w);
            Converter?.Convert(ref v, ref q);
            return (v, q);
        }

        public static (Vector3 pos, Quaternion rot) ToUnity(this Transform transform)
        {
            var v = new Vector3((float) transform.translation.x, (float) transform.translation.y,
                                (float) transform.translation.z);
            var q = new Quaternion((float) transform.rotation.x, (float) transform.rotation.y,
                                   (float) transform.rotation.z, (float) transform.rotation.w);
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

        private static string[] GetData(this object value)
        {
            switch (value)
            {
            case RosVector3 vector:
                return new[] {$"{vector.x:F3}, {vector.y:F3}, {vector.z:F3}"};
            case Point point:
                return new[] {$"{point.x:F3}, {point.y:F3}, {point.z:F3}"};
            case RosQuaternion quaternion:
                return new[] {$"{quaternion.x:F3}, {quaternion.y:F3}, {quaternion.z:F3}, {quaternion.w:F3}"};
            case Header header:
                return new[] {$"{header.stamp.secs}.{header.stamp.nsecs:000000000},{header.frame_id}"};
            case RosMessage message:
                return GetData(message);
            case string str:
                return new []{str};
            case IEnumerable arr:
                var builder = new StringBuilder();
                foreach (var o in arr)
                {
                    builder.Append($"{string.Join(",", o.GetData())}, ");
                }
                if (builder.Length >= 2) builder.Remove(builder.Length - 2, 2);
                return new[] {builder.ToString()};
            default:
                return new[] {value.ToString()};
            }
        }
    }
}