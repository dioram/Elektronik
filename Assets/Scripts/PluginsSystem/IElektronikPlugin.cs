using Elektronik.Common.Settings;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPlugin
    {
        string DisplayName { get; }
        string Description { get; }
        SettingsBag Settings { get; }
        void Start();
        void Stop();
        void Update();
    }
}