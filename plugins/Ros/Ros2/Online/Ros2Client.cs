#if !NO_ROS2DDS
using System;
using Elektronik.DataSources;
using Elektronik.PluginsSystem;
using Elektronik.RosPlugin.Common;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.Settings.Bags;
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
            converter.SetInitTRS(Vector3.zero, Quaternion.identity);
            RosMessageConvertExtender.Converter = converter;
        }
        
        #region IDataSourcePlugin

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
        public string Timestamp { get; }
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