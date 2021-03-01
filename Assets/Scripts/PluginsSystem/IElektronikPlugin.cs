using System;
using Elektronik.Common.Settings;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPlugin
    {
        string DisplayName { get; }
        string Description { get; }
        SettingsBag Settings { get; set; }
        ISettingsHistory SettingsHistory { get; }
        void Start();
        void Stop();
        void Update(float delta);
    }
}