using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UniRx;

namespace Elektronik.Settings
{
    /// <summary> Container for settings and their histories. </summary>
    /// <typeparam name="T"> Specific type of supported settings bag. </typeparam>
    public class SettingsHistory<T> : ISettingsHistory where T : SettingsBag
    {
        /// <summary> Constructor. Loads settings history from given file. </summary>
        /// <param name="filename">
        /// Name of file inside <see cref="SettingsRepository.Path"/> where history of this settings are stored.
        /// </param>
        /// <param name="maxCountOfRecentBags"> Max depth of history. </param>
        public SettingsHistory(string filename, int maxCountOfRecentBags = 10)
        {
            _maxCountOfRecentBags = maxCountOfRecentBags;
            _fileName = filename;
            Deserialize();
        }

        /// <summary> Adds new settings bag to history. </summary>
        /// <param name="recent"> Settings bag that will be added. </param>
        public void Add(T recent)
        {
            if (recent == null) return;
            if (!recent.Validate().Success) return;

            var existing = _recent.Items.Find(setting => setting.Equals(recent));
            if (existing != null)
            {
                _recent.Items.Remove(recent);
            }

            _recent.Items.Insert(0, recent);

            while (_recent.Items.Count > _maxCountOfRecentBags)
            {
                _recent.Items.RemoveAt(_recent.Items.Count - 1);
            }
        }

        #region ISettingsHistory

        /// <inheritdoc />
        public ReadOnlyCollection<SettingsBag> Recent =>
                _recent.Items.Select(s => s as SettingsBag).ToList().AsReadOnly();

        /// <inheritdoc />
        public void Add(SettingsBag recent)
        {
            Add((T)recent);
        }

        /// <inheritdoc />
        public void Save()
        {
            UniRxExtensions.StartOnMainThread(Serialize).Subscribe();
        }

        #endregion

        #region Private

        [Serializable]
        private class RecentItems
        {
            public List<T> Items = new List<T>();
        }

        private readonly string _fileName;
        private readonly int _maxCountOfRecentBags;
        private RecentItems _recent;

        private void Deserialize()
        {
            var pathToAppData = Path.Combine(SettingsRepository.Path, _fileName);
            if (File.Exists(pathToAppData))
            {
                _recent = JsonConvert.DeserializeObject<RecentItems>(File.ReadAllText(pathToAppData));
            }
            else
            {
                _recent = new RecentItems();
            }
        }

        private void Serialize()
        {
            var pathToAppData = Path.Combine(SettingsRepository.Path, _fileName);
            var fi = new FileInfo(pathToAppData);
            if (!(fi.Directory!.Exists)) fi.Directory.Create();
            File.WriteAllText(pathToAppData, JsonConvert.SerializeObject(_recent));
        }

        #endregion
    }
}