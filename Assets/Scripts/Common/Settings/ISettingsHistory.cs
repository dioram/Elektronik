using System.Collections.ObjectModel;

namespace Elektronik.Common.Settings
{
    public interface ISettingsHistory
    {
        void Add(SettingsBag recent);
        ReadOnlyCollection<SettingsBag> Recent { get; }
        void Save();
    }
}