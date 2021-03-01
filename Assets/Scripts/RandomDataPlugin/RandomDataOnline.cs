using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elektronik.Common.Containers;
using Elektronik.Common.Data.Converters;
using Elektronik.Common.Settings;
using Elektronik.PluginsSystem;

namespace Elektronik.RandomDataPlugin
{
    internal class RandomDataOnline : IDataSourceOnline
    {
        public string DisplayName => "Random data";

        public string Description => "Generates cloud of random points";

        public ICSConverter Converter { get; set; }

        public IContainerTree Data => _containers;

        public SettingsBag Settings
        {
            get => _settings;
            set => _settings = (RandomSettingsBag) value;
        }

        public ISettingsHistory SettingsHistory { get; } = new RandomSettingsHistory();

        public void Start()
        {
            _task = Task.Run(() =>
            {
                Thread.Sleep(500);
                _containers.Points.AddRange(Generator.GeneratePoints(0, 10000, _settings.Scale));
                _containers.Lines.AddRange(Generator.GenerateConnections(_containers.Points.ToList(), 2000));
                Thread.Sleep(100);

                for (int i = 0; i < 100; i++)
                {
                    _containers.Points.UpdateItems(
                        Generator.UpdatePoints(_containers.Points.ToList(), 3000, _settings.Scale));
                    Thread.Sleep(100);
                }
            });
        }

        public void Stop()
        {
            _task.Dispose();
        }

        public void Update(float delta)
        {
            // Do nothing
        }

        private Task _task;
        private RandomSettingsBag _settings = new RandomSettingsBag();
        private readonly RandomContainerTree _containers = new RandomContainerTree();
    }
}