using System.Collections.ObjectModel;
using Elektronik.Settings.Bags;

namespace Elektronik.Settings
{
    public interface ISettingsHistory
    {
        void Add(SettingsBag recent);
        ReadOnlyCollection<SettingsBag> Recent { get; }
        void Save();
    }
}