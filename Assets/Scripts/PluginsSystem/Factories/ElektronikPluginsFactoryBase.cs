using System;
using System.Globalization;
using System.IO;
using Elektronik.Data.Converters;
using Elektronik.Settings;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    /// <summary>
    /// Base class for all plugins factories. It implements some basic functions, such as settings and logo loading. 
    /// </summary>
    /// <typeparam name="TSettings"> Custom type of settings. </typeparam>
    public abstract class ElektronikPluginsFactoryBase<TSettings> : IElektronikPluginsFactory
            where TSettings : SettingsBag, new()
    {
        protected ElektronikPluginsFactoryBase()
        {
            _settingsHistory = new SettingsHistory<TSettings>($"{GetType().FullName}.json");
            if (_settingsHistory.Recent.Count > 0) _typedSettings = (TSettings) _settingsHistory.Recent[0].Clone();
            else _typedSettings = new TSettings();
        }

        /// <inheritdoc />
        public Texture2D Logo { get; private set; }

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public abstract string Description { get; }

        /// <inheritdoc />
        public ISettingsHistory SettingsHistory => _settingsHistory;

        /// <inheritdoc />
        public IElektronikPlugin Start(ICSConverter converter)
        {
            SaveSettings();
            return StartPlugin(_typedSettings, converter);
        }

        /// <inheritdoc />
        public void LoadLogo(string path)
        {
            if (!File.Exists(path)) return;
            Logo = new Texture2D(1, 1);
            Logo.LoadImage(File.ReadAllBytes(path));
        }

        /// <inheritdoc />
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

        /// <summary> This function actually instantiates plugin. </summary>
        /// <param name="settings"> Plugin's settings that are necessary for start. </param>
        /// <param name="converter"> Converter to unity's coordinates system. </param>
        /// <returns> Plugin. </returns>
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