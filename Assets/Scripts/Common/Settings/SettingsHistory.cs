using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    public class SettingsHistory<T> where T: SettingsBag
    {
        public ReadOnlyCollection<T> Recent => _recent.AsReadOnly();

        public SettingsHistory(int maxCountOfRecentFiles = 10)
        {
            _maxCountOfRecentFiles = maxCountOfRecentFiles;
            _fileName = $"{typeof(T).FullName}.json";
            
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
        private readonly int _maxCountOfRecentFiles;
        private List<T> _recent;
        
        private void Deserialize()
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, _fileName);
            var fi = new FileInfo(pathToAppData);
            if (fi.Directory.Exists && File.Exists(pathToAppData))
            {
                _recent = JsonUtility.FromJson<List<T>>(pathToAppData);
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
            File.WriteAllText(pathToAppData, JsonUtility.ToJson(_recent));
        }

        #endregion
    }
}