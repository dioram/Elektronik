using System;
using System.Globalization;
using Elektronik.Data.Converters;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.PluginsSystem
{
    public abstract class ElektronikPluginsFactoryBase<TSettings> : IElektronikPluginsFactory
            where TSettings : SettingsBag, new()
    {
        protected ElektronikPluginsFactoryBase()
        {
            _settingsHistory = new SettingsHistory<TSettings>($"{GetType().FullName}.json");
            if (_settingsHistory.Recent.Count > 0) TypedSettings = (TSettings) _settingsHistory.Recent[0].Clone();
            else TypedSettings = new TSettings();
        }

        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract string Version { get; }
        public ISettingsHistory SettingsHistory => _settingsHistory;

        public void SaveSettings()
        {
            TypedSettings.ModificationTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _settingsHistory.Add(TypedSettings.Clone());
            _settingsHistory.Save();
        }

        public SettingsBag Settings
        {
            get => TypedSettings;
            set
            {
                if (value is TSettings settings)
                {
                    TypedSettings = settings;
                }
                else
                {
                    throw new ArgumentException($"Wrong type of settings! " +
                                                $"Expected: {TypedSettings.GetType().Name}" +
                                                $"Got: {value.GetType().Name}");
                }
            }
        }

        public abstract IElektronikPlugin Start(ICSConverter converter);

        #region Protected

        protected TSettings TypedSettings;

        #endregion

        #region Private

        private readonly SettingsHistory<TSettings> _settingsHistory;

        #endregion
    }
}