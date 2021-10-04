using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using Elektronik.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace Elektronik.Settings
{
    public class SettingsHistory<T> : ISettingsHistory where T : SettingsBag
    {
        public SettingsHistory(string filename, int maxCountOfRecentFiles = 10)
        {
            _maxCountOfRecentFiles = maxCountOfRecentFiles;
            _fileName = filename;
            Deserialize();
        }

        public ReadOnlyCollection<SettingsBag> Recent =>
                _recent.Items.Select(s => s as SettingsBag).ToList().AsReadOnly();

        public void Add(SettingsBag recent)
        {
            Add((T) recent);
        }

        public void Add(T recent)
        {
            if (recent == null) return;
            if (!recent.Validate().Success) return;

            T existing = _recent.Items.Find(setting => setting.Equals(recent));
            if (existing != null)
            {
                _recent.Items.Remove(recent);
            }

            _recent.Items.Insert(0, recent);

            while (_recent.Items.Count > _maxCountOfRecentFiles)
            {
                _recent.Items.RemoveAt(_recent.Items.Count - 1);
            }
        }

        public void Save()
        {
            MainThreadInvoker.Instance.Enqueue(Serialize);
        }

        #region Private definitions

        [Serializable]
        private class RecentItems
        {
            public List<T> Items = new List<T>();
        }

        private readonly string _fileName;
        private readonly int _maxCountOfRecentFiles;
        private RecentItems _recent;

        private string SavePath => Application.persistentDataPath;

        private void Deserialize()
        {
            string pathToAppData;
            try
            {
                pathToAppData = Path.Combine(SavePath, _fileName);
            }
            catch (SecurityException)
            {
                Console.WriteLine("Security exception was raised. " +
                                  "Probably because you are running UnityEngine.dll outside Unity player.");
                pathToAppData = _fileName;
            }

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
            string pathToAppData;
            try
            {
                pathToAppData = Path.Combine(SavePath, _fileName);
            }
            catch (SecurityException)
            {
                Console.WriteLine("Security exception was raised. " +
                                  "Probably because you are running UnityEngine.dll outside Unity player.");
                pathToAppData = _fileName;
            }
            var fi = new FileInfo(pathToAppData);
            if (!fi.Directory.Exists) fi.Directory.Create();
            File.WriteAllText(pathToAppData, JsonConvert.SerializeObject(_recent));
        }

        #endregion
    }
}