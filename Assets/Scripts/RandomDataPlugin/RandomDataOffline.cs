using System;
using System.Linq;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Settings;
using Elektronik.Offline;
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
            if (PlayerEvents == null)
            {
                throw new NullReferenceException($"No player events set for {nameof(RandomDataOffline)}");
            }

            PlayerEvents.Play += delegate { _containers.Points.AddRange(Generator.GeneratePoints(0, 10000, _settings.Scale)); };
            PlayerEvents.NextKeyFrame += delegate
            {
                _containers.Points.UpdateItems(Generator.UpdatePoints(_containers.Points.ToList(), 2000, _settings.Scale));
            };
            PlayerEvents.Pause += delegate
            {
                _containers.Points.UpdateItems(Generator.UpdatePoints(_containers.Points.ToList(), 2000, _settings.Scale));
            };
        }

        public void Stop()
        {
            // Do nothing
        }

        public void Update()
        {
            // Do nothing
        }

        public IPlayerEvents PlayerEvents { get; set; }
        public int AmountOfFrames { get; } = 10;
        
        private RandomSettingsBag _settings = new RandomSettingsBag();
        private readonly RandomContainerTree _containers = new RandomContainerTree();
    }
}