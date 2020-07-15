using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Elektronik.Common.Settings
{
    public class SettingsStore<T> : MonoBehaviour
        where T : IComparable<T>
    {
        public int maxCountOfRecentFiles = 20;
        public List<T> Recent { get; private set; }

        private JsonSerializerSettings m_settings;

        private void Awake()
        {
            Recent = new List<T>(maxCountOfRecentFiles + 1); // +1 because of first add and second remove
            m_settings = new JsonSerializerSettings();
            m_settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        }

        public void Add(T recent)
        {
            T existing = Recent.Find(setting => setting.CompareTo(recent) == 0);
            if (existing != null)
            {
                Recent.Remove(recent);
            }
            Recent.Insert(0, recent);
            if (Recent.Count > maxCountOfRecentFiles)
                Recent.RemoveAt(Recent.Count - 1);
        }

        public void Deserialize(string filename)
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, filename);
            var fi = new FileInfo(pathToAppData);
            if (fi.Directory.Exists && File.Exists(pathToAppData))
            {
                Recent = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(pathToAppData), m_settings);
            }
            else
            {
                Recent = new List<T>();
            }
            Debug.Log(Recent.GetType());
        }

        public void Serialize(string filename)
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, filename);
            var fi = new FileInfo(pathToAppData);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            Debug.Log($"Serialization to:{Environment.NewLine}{pathToAppData}");
            File.WriteAllText(pathToAppData, JsonConvert.SerializeObject(Recent, m_settings));
        }
    }
}