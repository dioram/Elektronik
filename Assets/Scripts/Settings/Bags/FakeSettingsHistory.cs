using System;
using System.Collections.ObjectModel;

namespace Elektronik.Settings.Bags
{
    public class FakeSettingsHistory : ISettingsHistory
    {
        public void Add(SettingsBag recent)
        {
            // Do nothing
        }

        public ReadOnlyCollection<SettingsBag> Recent { get; } =
            new ReadOnlyCollection<SettingsBag>(Array.Empty<SettingsBag>());

        public void Save()
        {
            // Do nothing
        }
    }
}