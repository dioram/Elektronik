using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.RosPlugin.Common.RosMessages;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online.Handlers
{
    public class OdometryHandler : MessageHandler
    {
        private readonly IContainer<SlamTrackedObject> _container;

        public OdometryHandler(IContainer<SlamTrackedObject> container)
        {
            _container = container;
        }

        public override void Handle(Ros2Message message)
        {
            var odometry = message.CastTo<OdometryMessage>();
            if (odometry == null) return;
            var pos = new Vector3((float) odometry.pos_x, (float) odometry.pos_y, (float) odometry.pos_z);
            var rot = new Quaternion((float) odometry.rot_x, (float) odometry.rot_y, (float) odometry.rot_z,
                                     (float) odometry.rot_w);

            RosMessageConvertExtender.Converter?.Convert(ref pos, ref rot);
            SlamTrackedObject obj = new SlamTrackedObject(0, pos, rot);

            if (_container.Count == 0) _container.Add(obj);
            else _container.Update(obj);
            message.Dispose();
        }
    }
}