using System.Collections.ObjectModel;

namespace Elektronik.Settings
{
    public interface ISettingsHistory
    {
        void Add(SettingsBag recent);
        ReadOnlyCollection<SettingsBag> Recent { get; }
        void Save();
    }
    
    public static class SettingsRepository
    {
        public static string Path = "./";
    }
}