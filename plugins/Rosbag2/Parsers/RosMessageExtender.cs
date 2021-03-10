using System;
using System.Linq;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Nav;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using RosMessage = RosSharp.RosBridgeClient.Message;

namespace Elektronik.Rosbag2.Parsers
{
    public static class RosMessageExtender
    {
        public static SlamTrackedObject ToTrackedObject(this RosMessage rosMessage, ICSConverter converter)
        {
            var field = rosMessage.GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.PropertyType == typeof(Pose));
            if (field != null) 
                return FromPose(((Pose) field.GetValue(rosMessage)), converter);

            field = rosMessage.GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.PropertyType == typeof(PoseWithCovariance));
            if (field != null) 
                return FromPose(((PoseWithCovariance) field.GetValue(rosMessage)).pose, converter);

            field = rosMessage.GetType()
                    .GetProperties()
                    .FirstOrDefault(f => f.PropertyType == typeof(PoseWithCovarianceStamped));
            if (field != null)
                return FromPose(((PoseWithCovarianceStamped) field.GetValue(rosMessage)).pose.pose, converter);

            throw new NotSupportedException("Can't convert to SlamTrackedObject message without pose.");
        }

        private static SlamTrackedObject FromPose(Pose pose, ICSConverter converter)
        {
            var (v, q) = pose.ToUnity(converter);
            return new SlamTrackedObject
            {
                Id = 0,
                Position = v,
                Rotation = q,
            };
        }

        public static (Vector3, Quaternion) ToUnity(this Pose pose, ICSConverter converter)
        {
            Vector3 v = new Vector3((float) pose.position.x, (float) pose.position.y, (float) pose.position.z);
            Quaternion q = new Quaternion((float) pose.orientation.x, (float) pose.orientation.y,
                                          (float) pose.orientation.z,
                                          (float) pose.orientation.w);
            converter.Convert(ref v, ref q);
            return (v, q);
        }
    }
}