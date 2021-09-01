#if !NO_ROS2DDS
using Elektronik.Data;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2Client : IDataSourcePluginOnline
    {
        public Ros2Client(Ros2Settings settings)
        {
            _container = new Ros2OnlineContainerTree(settings, "TMP");
            Data = _container;
            var converter = new RosConverter();
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = converter;
        }
        
        #region IDataSourceOnline

        public ISourceTree Data { get; }

        public void Dispose()
        {
            _container.Dispose();
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        #endregion

        #region Private
        
        private readonly Ros2OnlineContainerTree _container;

        #endregion
    }
}
#endif