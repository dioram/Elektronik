using System;
using System.Linq;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Presenters;
using Elektronik.Common.Settings;
using Elektronik.PluginsSystem;

namespace Elektronik.RandomDataPlugin
{
    internal class RandomDataOffline : IDataSourceOffline
    {
        public string DisplayName => "Random data";
        public string Description => "Generates cloud of random points";

        public ICSConverter Converter { get; set; }
        
        public IContainerTree Data => _containers;

        public SettingsBag Settings => _settings;

        public void Start()
        {
            _containers.Points.AddRange(Generator.GeneratePoints(0, 10000, _settings.Scale));
        }

        public void Stop()
        {
            // Do nothing
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        public int AmountOfFrames { get; } = 10;
        public int CurrentTimestamp { get; } = 0;
        public int CurrentPosition { get; set; }

        public void Play()
        {
            // Do nothing
        }

        public void Pause()
        {
            // Do nothing
        }

        public void StopPlaying()
        {
            // Do nothing
        }

        public void PreviousKeyFrame()
        {
            // Do nothing
        }

        public void NextKeyFrame()
        {
            _containers.Points.UpdateItems(Generator.UpdatePoints(_containers.Points.ToList(), 2000, _settings.Scale));
        }

        public event Action Finished;

        public DataPresenter PresentersChain { get; } = null;

        private RandomSettingsBag _settings = new RandomSettingsBag();
        private readonly RandomContainerTree _containers = new RandomContainerTree();
    }
}