using System;
using System.Globalization;
using System.IO;
using Elektronik.Data.Converters;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.PluginsSystem
{
    public abstract class ElektronikPluginsFactoryBase<TSettings> : IElektronikPluginsFactory
            where TSettings : SettingsBag, new()
    {
        protected ElektronikPluginsFactoryBase()
        {
            _settingsHistory = new SettingsHistory<TSettings>($"{GetType().FullName}.json");
            if (_settingsHistory.Recent.Count > 0) _typedSettings = (TSettings) _settingsHistory.Recent[0].Clone();
            else _typedSettings = new TSettings();
        }

        public Texture2D Logo { get; private set; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public ISettingsHistory SettingsHistory => _settingsHistory;

        public IElektronikPlugin Start(ICSConverter converter)
        {
            SaveSettings();
            return StartPlugin(_typedSettings, converter);
        }

        public void LoadLogo(string path)
        {
            if (!File.Exists(path)) return;
            Logo = new Texture2D(1, 1);
            Logo.LoadImage(File.ReadAllBytes(path));
        }

        public SettingsBag Settings
        {
            get => _typedSettings;
            set
            {
                if (value is TSettings settings)
                {
                    _typedSettings = settings;
                }
                else
                {
                    throw new ArgumentException($"Wrong type of settings! " +
                                                $"Expected: {_typedSettings.GetType().Name}" +
                                                $"Got: {value.GetType().Name}");
                }
            }
        }

        #region Protected

        protected abstract IElektronikPlugin StartPlugin(TSettings settings, ICSConverter converter);

        #endregion

        #region Private

        private TSettings _typedSettings;
        private readonly SettingsHistory<TSettings> _settingsHistory;

        private void SaveSettings()
        {
            _typedSettings.ModificationTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _settingsHistory.Add(_typedSettings.Clone());
            _settingsHistory.Save();
        }

        #endregion
    }
}