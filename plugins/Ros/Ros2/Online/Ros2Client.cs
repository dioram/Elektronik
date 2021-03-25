using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2Client : DataSourceBase<Ros2Settings>, IDataSourceOnline
    {
        public Ros2Client()
        {
            _container = new Ros2OnlineContainerTree("TMP");
            Data = _container;
        }
        #region IDataSourceOnline

        public override string DisplayName => "ROS2 listener";
        public override string Description => "Client for ROS2 network.";
        
        public override void Start()
        {
            _container.Init(TypedSettings);
            Converter = new RosConverter();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity, Vector3.one * TypedSettings.Scale);
            RosMessageConvertExtender.Converter = Converter;
        }

        public override void Stop()
        {
            _container.Reset();
        }

        public override void Update(float delta)
        {
            // Do nothing
        }

        #endregion

        #region Private
        
        private readonly Ros2OnlineContainerTree _container;

        #endregion
    }
}