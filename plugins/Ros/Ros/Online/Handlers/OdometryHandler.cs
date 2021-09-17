﻿using Elektronik.Data.PackageObjects;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Nav;

namespace Elektronik.RosPlugin.Ros.Online.Handlers
{
    public class OdometryHandler : MessageHandler<Odometry, IContainer<SlamTrackedObject>>
    {
        public OdometryHandler(ISourceTreeNode container, RosSocket socket, string topic) : base(
            container, socket, topic)
        {
        }

        protected override void Handle(Odometry message)
        {
            if (Container?.Count == 0) Container.Add(message.GetPose()!.ToTracked());
            else Container?.Update(message.GetPose()!.ToTracked());
        }
    }
}