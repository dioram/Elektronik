using System;
using System.Linq;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Settings;
using Elektronik.Offline;
using Elektronik.PluginsSystem;
using SlamPointsContainer = Elektronik.Common.Containers.NotMono.SlamPointsContainer;
using SlamLinesContainer = Elektronik.Common.Containers.NotMono.SlamLinesContainer;

namespace Elektronik.RandomDataPlugin
{
    internal class RandomDataOffline : IDataSourceOffline
    {
        public string DisplayName => "Random data";
        public string Description => "Generates cloud of random points";

        public IContainerTree[] Children { get; } = new IContainerTree[2];


        public RandomDataOffline()
        {
            Children[0] = _points;
            Children[1] = _lines;
        }

        public void SetActive(bool active)
        {
            foreach (var child in Children)
            {
                child.SetActive(active);
            }
        }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
            }
        }

        public void SetRenderers(ICloudRenderer[] renderers)
        {
            foreach (var child in Children)
            {
                child.SetRenderers(renderers);
            }
        }

        public ICSConverter Converter { get; set; }

        public SettingsBag Settings
        {
            get => _settings;
            set => _settings = (RandomSettingsBag) value;
        }

        public Type RequiredSettingsType => typeof(RandomSettingsBag);

        public void Start()
        {
            if (PlayerEvents == null)
            {
                throw new NullReferenceException($"No player events set for {nameof(RandomDataOffline)}");
            }

            PlayerEvents.Play += delegate { _points.AddRange(Generator.GeneratePoints(0, 10000, _settings.Scale)); };
            PlayerEvents.NextKeyFrame += delegate
            {
                _points.UpdateItems(Generator.UpdatePoints(_points.ToList(), 2000, _settings.Scale));
            };
            PlayerEvents.Pause += delegate
            {
                _points.UpdateItems(Generator.UpdatePoints(_points.ToList(), 2000, _settings.Scale));
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


        private SlamPointsContainer _points = new SlamPointsContainer();
        private SlamLinesContainer _lines = new SlamLinesContainer();
        private RandomSettingsBag _settings;
    }
}