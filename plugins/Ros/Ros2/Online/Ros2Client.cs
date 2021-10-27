#if !NO_ROS2DDS
using Elektronik.DataSources;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Online
{
    public class Ros2Client : IDataSourcePlugin
    {
        public Ros2Client(string displayName, Texture2D? logo, Ros2Settings settings)
        {
            DisplayName = displayName;
            Logo = logo;
            _container = new Ros2OnlineContainerTree(settings, "TMP");
            Data = _container;
            var converter = new RosConverter();
            RosMessageConvertExtender.Converter = converter;
        }
        
        #region IDataSourcePlugin
        
        public IDataSource Data { get; }

        public void Dispose()
        {
            _container.Dispose();
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        public string DisplayName { get; }
        public SettingsBag? Settings => null;
        public Texture2D? Logo { get; }

        #endregion

        #region Private
        
        private readonly Ros2OnlineContainerTree _container;

        #endregion
    }
}
#endif