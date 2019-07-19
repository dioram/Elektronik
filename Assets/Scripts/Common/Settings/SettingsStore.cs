using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    public class SettingsStore<T> : MonoBehaviour
        where T : IComparable<T>
    {
        public int maxCountOfRecentFiles = 20;
        
        public List<T> Recent { get; private set; }

        private void Awake()
        {
            Recent = new List<T>(maxCountOfRecentFiles + 1); // +1 because of first add and second remove
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
                string str = File.ReadAllText(pathToAppData);
                using (var sw = new StringReader(str))
                using (var reader = new JsonTextReader(sw))
                {
                    var settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.TypeNameHandling = TypeNameHandling.Auto;
                    settings.Formatting = Formatting.Indented;
                    settings.Converters.Add(new IPAddressConverter());
                    settings.Converters.Add(new IPEndPointConverter());
                    Recent = JsonConvert.DeserializeObject<List<T>>(str, settings);
                }
            }
            else
            {
                Recent = new List<T>();
            }
        }

        public void Serialize(string filename)
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, filename);
            var fi = new FileInfo(pathToAppData);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            Debug.Log($"Serialization to:{Environment.NewLine}{pathToAppData}");
            
            using (var file = File.Open(pathToAppData, FileMode.Create))
            using (var writer = new StreamWriter(file))
            {
                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.TypeNameHandling = TypeNameHandling.Auto;
                settings.Formatting = Formatting.Indented;
                settings.Converters.Add(new IPAddressConverter());
                settings.Converters.Add(new IPEndPointConverter());
                string json = JsonConvert.SerializeObject(Recent, settings);
                writer.Write(json);
            }
        }
    }
}