using System;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.Containers;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using UnityEngine;
using Pose = RosSharp.RosBridgeClient.MessageTypes.Geometry.Pose;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using RosMessage = RosSharp.RosBridgeClient.Message;
using RosVector3 = RosSharp.RosBridgeClient.MessageTypes.Geometry.Vector3;
using Time = RosSharp.RosBridgeClient.MessageTypes.Std.Time;

namespace Elektronik.RosPlugin.Common.RosMessages
{
    public static class RosMessageConvertExtender
    {
        public static ICSConverter? Converter;

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

        public static ImagePresenter.ImageData ToImageData(this Image image)
            => new((int)image.width, (int)image.height, image.encoding, image.data);
        

        public static TextureFormat GetTextureFormat(string encoding) =>
                encoding switch
                {
                    "rgb8" => TextureFormat.RGB24,
                    "rgba8" => TextureFormat.RGBA32,
                    "rgb16" => TextureFormat.RGB48,
                    "rgba16" => TextureFormat.RGBA64,
                    "bgra8" => TextureFormat.BGRA32,
                    "mono8" => TextureFormat.Alpha8,
                    "mono16" => TextureFormat.R16,
                    _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding,
                                                               "This type of encoding is not supported.")
                };
    }
}