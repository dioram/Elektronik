using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    public class SettingsHistory<T> where T: SettingsBag
    {
        public ReadOnlyCollection<T> Recent => _recent.AsReadOnly();

        public SettingsHistory(string fileName, int maxCountOfRecentFiles = 10)
        {
            _maxCountOfRecentFiles = maxCountOfRecentFiles;
            _fileName = fileName;
            _settings = new JsonSerializerSettings
            {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            
            Deserialize();
        }

        public void Add(T recent)
        {
            T existing = _recent.Find(setting => setting.Equals(recent));
            if (existing != null)
            {
                _recent.Remove(recent);
            }
            _recent.Insert(0, recent);
            
            while (_recent.Count > _maxCountOfRecentFiles)
            {
                _recent.RemoveAt(_recent.Count - 1);
            }
               
        }

        public void Save()
        {
            Serialize();
        }

        #region Private definitions
        
        private readonly string _fileName;
        private readonly JsonSerializerSettings _settings;
        private readonly int _maxCountOfRecentFiles;
        private List<T> _recent;
        
        private void Deserialize()
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, _fileName);
            var fi = new FileInfo(pathToAppData);
            if (fi.Directory.Exists && File.Exists(pathToAppData))
            {
                _recent = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(pathToAppData), _settings);
            }
            else
            {
                _recent = new List<T>();
            }
        }
        
        private void Serialize()
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, _fileName);
            var fi = new FileInfo(pathToAppData);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            File.WriteAllText(pathToAppData, JsonConvert.SerializeObject(_recent, _settings));
        }

        #endregion
    }
}