#if !NO_ROS2DDS
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online.Handlers
{
    public class PoseStampedHandler : MessageHandler
    {
        private readonly IContainer<SlamTrackedObject> _container;

        public PoseStampedHandler(IContainer<SlamTrackedObject> container)
        {
            _container = container;
        }

        public override void Handle(Ros2Message message)
        {
            var pose = message.CastTo<PoseStampedMessage>();
            if (pose == null) return;
            var pos = new Vector3((float) pose.pos_x, (float) pose.pos_y, (float) pose.pos_z);
            var rot = new Quaternion((float) pose.rot_x, (float) pose.rot_y, (float) pose.rot_z, (float) pose.rot_w);

            RosMessageConvertExtender.Converter?.Convert(ref pos, ref rot);
            SlamTrackedObject obj = new SlamTrackedObject(0, pos, rot);

            if (_container.Count == 0) _container.Add(obj);
            else _container.Update(obj);
            message.Dispose();
        }
    }
}
#endif