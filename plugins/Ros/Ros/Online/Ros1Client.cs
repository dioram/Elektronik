using System;
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
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = converter;
        }

        #region IDataSourcePlayer

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void StopPlaying()
        {
            throw new NotImplementedException();
        }

        public void PreviousKeyFrame()
        {
            throw new NotImplementedException();
        }

        public void NextKeyFrame()
        {
            throw new NotImplementedException();
        }

        public void PreviousFrame()
        {
            throw new NotImplementedException();
        }

        public void NextFrame()
        {
            throw new NotImplementedException();
        }

        public ISourceTreeNode Data { get; }
        public int AmountOfFrames { get; }
        public string Timestamp { get; private set; } = "";
        public int Position { get; set; }
        public float Speed { get; set; }
        public bool IsPlaying { get; }
        public event Action? OnPlayingStarted;
        public event Action? OnPaused;
        public event Action<int>? OnPositionChanged;
        public event Action<int>? OnAmountOfFramesChanged;
        public event Action<string>? OnTimestampChanged;
        public event Action? OnRewindStarted;
        public event Action? OnRewindFinished;
        public event Action? OnFinished;

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