using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Elektronik.Settings
{
    public class SettingsHistory<T> : ISettingsHistory where T : SettingsBag
    {
        [Serializable]
        private class RecentItems
        {
            public List<T> Items = new List<T>();
        }

        public void Add(SettingsBag recent)
        {
            Add((T)recent);
        }

        public ReadOnlyCollection<SettingsBag> Recent => _recent.Items.Select(s => s as SettingsBag).ToList().AsReadOnly();

        public SettingsHistory(int maxCountOfRecentFiles = 10)
        {
            _maxCountOfRecentFiles = maxCountOfRecentFiles;
            _fileName = $"{typeof(T).FullName}.json";
            
            Deserialize();
        }

        public void Add(T recent)
        {
            if (recent == null) return;
            if (!recent.Validate()) return;
            
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
            Serialize();
        }

        #region Private definitions
        
        private readonly string _fileName;
        private readonly int _maxCountOfRecentFiles;
        private RecentItems _recent;
        
        private void Deserialize()
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, _fileName);
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
            string pathToAppData = Path.Combine(Application.persistentDataPath, _fileName);
            var fi = new FileInfo(pathToAppData);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            File.WriteAllText(pathToAppData, JsonConvert.SerializeObject(_recent));
        }

        #endregion
    }
}