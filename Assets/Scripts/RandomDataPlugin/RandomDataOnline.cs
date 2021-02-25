using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Common.Clouds;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Settings;
using Elektronik.PluginsSystem;
using SlamPointsContainer = Elektronik.Common.Containers.NotMono.SlamPointsContainer;
using SlamLinesContainer = Elektronik.Common.Containers.NotMono.SlamLinesContainer;

namespace Elektronik.RandomDataPlugin
{
    internal class RandomDataOnline : IDataSourceOnline
    {
        public string DisplayName => "Random data";
        public string Description => "Generates cloud of random points";

        public RandomDataOnline()
        {
            Children[0] = _points;
            Children[1] = _lines;
        }

        public IContainerTree[] Children { get; } = new IContainerTree[2];
        
        public void SetActive(bool active)
        {
            foreach (var child in Children)
            {
                child.SetActive(true);
            }
        }

        public void Clear()
        {
            foreach (var child in Children)
            {
                child.Clear();
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
            _task = Task.Run(() =>
            {
                Thread.Sleep(500);
                _points.AddRange(Generator.GeneratePoints(0, 10000, _settings.Scale));
                _lines.AddRange(Generator.GenerateConnections(_points.ToList(), 2000));
                Thread.Sleep(100);

                for (int i = 0; i < 100; i++)
                {
                    _points.UpdateItems(Generator.UpdatePoints(_points.ToList(), 3000, _settings.Scale));
                    Thread.Sleep(100);
                }
            });
        }

        public void Stop()
        {
            _task.Dispose();
        }

        public void Update()
        {
            // Do nothing
        }

        private Task _task;
        private SlamPointsContainer _points = new SlamPointsContainer();
        private SlamLinesContainer _lines = new SlamLinesContainer();
        private RandomSettingsBag _settings;
    }
}