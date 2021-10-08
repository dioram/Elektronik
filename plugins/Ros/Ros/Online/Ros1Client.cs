using Elektronik.DataSources;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.Settings;
using RosSharp.RosBridgeClient.MessageTypes.Rosapi;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros.Online
{
    public class Ros1Client : IDataSourcePlugin
    {
        public Ros1Client(string displayName, Texture2D? logo, Ros1Settings settings)
        {
            DisplayName = displayName;
            Logo = logo;
            _container = new RosOnlineContainerTree(settings, "TMP");
            Data = _container;
            var converter = new RosConverter();
            RosMessageConvertExtender.Converter = converter;
        }

        #region IDataSourcePlayer

        public ISourceTreeNode Data { get; }

        public void Dispose()
        {
            _container.Dispose();
        }

        public void Update(float delta)
        {
            if (_topicsUpdateTimeout < 0)
            {
                _topicsUpdateTimeout = TopicsUpdateTimeout;
                _container.Socket?.CallService<TopicsRequest, TopicsResponse>("/rosapi/topics", _container.UpdateTopics,
                                                                              new TopicsRequest());
            }

            _topicsUpdateTimeout -= delta;
        }

        public string DisplayName { get; }
        public SettingsBag? Settings => null;
        public Texture2D? Logo { get; }

        #endregion

        #region Private

        private const float TopicsUpdateTimeout = 0.2f;
        private float _topicsUpdateTimeout = 0;

        private readonly RosOnlineContainerTree _container;

        #endregion
    }
}