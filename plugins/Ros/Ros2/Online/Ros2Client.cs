#if !NO_ROS2DDS
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2Client : DataSourcePluginBase<Ros2Settings>, IDataSourcePluginOnline
    {
        public Ros2Client()
        {
            _container = new Ros2OnlineContainerTree("TMP");
            Data = _container;
        }
        
        #region IDataSourceOnline

        public override string DisplayName => "ROS2 listener";
        public override string Description => "Client for " +
                "<#7f7fe5><u><link=\"https://docs.ros.org/en/foxy/index.html\">ROS2</link></u></color> network.";
        
        public override void Start()
        {
            _container.Init(TypedSettings);
            Converter = new RosConverter();
            Converter.SetInitTRS(Vector3.zero, Quaternion.identity);
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
#else

#endif