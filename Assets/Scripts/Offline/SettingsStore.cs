using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace Elektronik.Offline
{
    public class SettingsStore : MonoBehaviour
    {
        public int maxCountOfRecentFiles = 20;

        public List<RecentFile> RecentFiles { get; private set; }

        private void Awake()
        {
            RecentFiles = new List<RecentFile>(maxCountOfRecentFiles + 1); // +1 because of first add and second remove
        }

        private void Start()
        {
            Deserialize();
        }

        public void Add(RecentFile recentFile)
        {
            RecentFiles.Remove(recentFile);
            RecentFiles.Insert(0, recentFile);
            if (RecentFiles.Count > maxCountOfRecentFiles)
                RecentFiles.RemoveAt(RecentFiles.Count - 1);
        }

        public void Deserialize()
        {
            string pathToAppData = Application.dataPath + @"/offline/settings.dat";
            IFormatter formatter = new BinaryFormatter();
            if (File.Exists(pathToAppData))
            {
                using (var file = File.Open(pathToAppData, FileMode.Open))
                {
                    RecentFiles = (List<RecentFile>)formatter.Deserialize(file);
                }
            }
        }

        public void Serialize()
        {
            string pathToAppData = Application.dataPath + @"/offline/settings.dat";
            Debug.LogFormat("Serialization to:{0}{1}", Environment.NewLine, pathToAppData);
            IFormatter formatter = new BinaryFormatter();
            using (var file = File.Open(pathToAppData, FileMode.OpenOrCreate))
            {
                formatter.Serialize(file, RecentFiles);
            }
        }
    }
}