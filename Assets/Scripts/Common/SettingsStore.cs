using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Elektronik.Common
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
            FileInfo fi = new FileInfo(pathToAppData);
            IFormatter formatter = new BinaryFormatter();
            if (fi.Directory.Exists && File.Exists(pathToAppData))
            {
                using (var file = File.Open(pathToAppData, FileMode.Open))
                {
                    Recent = (List<T>)formatter.Deserialize(file);
                }
            }
        }

        public void Serialize(string filename)
        {
            string pathToAppData = Path.Combine(Application.persistentDataPath, filename);
            FileInfo fi = new FileInfo(pathToAppData);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            Debug.LogFormat("Serialization to:{0}{1}", Environment.NewLine, pathToAppData);
            IFormatter formatter = new BinaryFormatter();
            using (var file = File.Open(pathToAppData, FileMode.OpenOrCreate))
            {
                formatter.Serialize(file, Recent);
            }
        }
    }
}