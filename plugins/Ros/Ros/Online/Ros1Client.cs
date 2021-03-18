using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.Settings;
using RosSharp.RosBridgeClient.MessageTypes.Rosapi;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros.Online
{
    public class Ros1Client : DataSourceBase<AddressPortScaleSettingsBag>, IDataSourceOnline
    {
        public Ros1Client()
        {
            _container = new RosOnlineContainerTree("TMP");
            Data = _container;
        }

        #region IDataSourceOnline

        public override string DisplayName => "ROS listener";

        public override string Description =>
                "Client for ROS1 network. Needs to be used with Rosbridge. " +
                "See: http://wiki.ros.org/rosbridge_suite/Tutorials/RunningRosbridge";

        public override void Start()
        {
            _container.Init(TypedSettings);
            Converter = new RosConverter();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);
            RosMessageConvertExtender.Converter = Converter;
        }

        public override void Stop()
        {
        }

        public override void Update(float delta)
        {
            if (_topicsUpdateTimeout < 0)
            {
                _topicsUpdateTimeout = TopicsUpdateTimeout;
                _container.Socket?.CallService<TopicsRequest, TopicsResponse>("/rosapi/topics", _container.UpdateTopics,
                                                                              new TopicsRequest());
            }

            _topicsUpdateTimeout -= delta;
        }

        #endregion

        #region Private

        private const float TopicsUpdateTimeout = 0.2f;
        private float _topicsUpdateTimeout = 0;

        private readonly RosOnlineContainerTree _container;

        #endregion
    }
}