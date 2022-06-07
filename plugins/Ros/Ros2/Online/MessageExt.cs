#if !NO_ROS2DDS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Elektronik.DataObjects;
using Elektronik.RosPlugin.Common.RosMessages;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public static class MessageExt
    {
        public static string GetRosType(string type)
        {
            var path = type.Split(':').Where(s => !string.IsNullOrEmpty(s)).ToList();
            path.Remove("dds_");
            path[path.Count - 1] = path.Last().Substring(0, path.Last().Length - 1);
            return string.Join("/", path);
        }
        
        public static string GetRosTopic(string name)
        {
            return string.Join("/", name.Split('/').Skip(1));
        }
        
        public static string GetDdsTopic(string name)
        {
            return string.Join("/", new List<string>{"rt"}.Concat(name.Split('/')));
        }
        
        public static string GetDdsType(string type)
        {
            var path = type.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToList();
            path.Insert(path.Count - 1, "dds_");
            path[path.Count - 1] += "_";
            return string.Join("::", path);
        }

        public static T? CastTo<T>(this Ros2Message from) where T: Ros2Message
        {
            MethodInfo? cPtrGetter = from.GetType().GetMethod("getCPtr", BindingFlags.NonPublic
                                                              | BindingFlags.Static);
            return cPtrGetter == null
                    ? default
                    : (T) Activator.CreateInstance
                    (
                        typeof(T),
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        new object[] {((HandleRef) cPtrGetter.Invoke(null, new object[] {from})).Handle, true},
                        null
                    );
        }
        
        public static SlamPoint[] ToSlamPoints(this PointCloud2Message cloud)
        {
            var data = cloud.data.ToArray();
            var res = new SlamPoint[data.Length / cloud.point_step];

            var xOffset = cloud.fields.First(f => f.Name == "x").Offset;
            var yOffset = cloud.fields.First(f => f.Name == "y").Offset;
            var zOffset = cloud.fields.First(f => f.Name == "z").Offset;
            var intensityOffset = cloud.fields.FirstOrDefault(f => f.Name == "intensity")?.Offset ??
                    cloud.point_step + 1;
            var rgbOffset = cloud.fields.FirstOrDefault(f => f.Name == "rgb")?.Offset ?? cloud.point_step + 1;
            var rgbaOffset = cloud.fields.FirstOrDefault(f => f.Name == "rgba")?.Offset ??
                    cloud.point_step + 1;

            for (int i = 0; i < data.Length / cloud.point_step; i++)
            {
                var x = BitConverter.ToSingle(data, (int) (i * cloud.point_step + xOffset));
                var y = BitConverter.ToSingle(data, (int) (i * cloud.point_step + yOffset));
                var z = BitConverter.ToSingle(data, (int) (i * cloud.point_step + zOffset));

                Color color = Color.black;
                if (intensityOffset < cloud.point_step)
                {
                    var intensity = BitConverter.ToSingle(data, (int) (i * cloud.point_step + intensityOffset));
                    color = RosMessageConvertExtender.ColorFromIntensity(intensity);
                }
                else if (rgbOffset < cloud.point_step)
                {
                    byte r = data[i * cloud.point_step + rgbOffset + 0];
                    byte g = data[i * cloud.point_step + rgbOffset + 1];
                    byte b = data[i * cloud.point_step + rgbOffset + 2];
                    color = new Color(r / (float) byte.MaxValue, g / (float) byte.MaxValue, b / (float) byte.MaxValue);
                }
                else if (rgbaOffset < cloud.point_step)
                {
                    byte a = data[i * cloud.point_step + rgbaOffset + 0];
                    byte r = data[i * cloud.point_step + rgbaOffset + 1];
                    byte g = data[i * cloud.point_step + rgbaOffset + 2];
                    byte b = data[i * cloud.point_step + rgbaOffset + 3];
                    color = new Color(r / (float) byte.MaxValue, g / (float) byte.MaxValue, b / (float) byte.MaxValue,
                                      a / (float) byte.MaxValue);
                }

                var pos = new Vector3(x, y, z);
                RosMessageConvertExtender.Converter?.Convert(ref pos);
                res[i] = new SlamPoint(i, pos, color);
            }

            return res;
        }
    }
}
#endif