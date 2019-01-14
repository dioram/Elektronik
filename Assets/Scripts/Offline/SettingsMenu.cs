using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SimpleFileBrowser;
using UnityEditor;
using System.Linq;
using Elektronik.Common;
using System.IO;
using System;
using System.Threading;

namespace Elektronik.Offline
{
    public class SettingsMenu : MonoBehaviour
    {
        public RecentFileListBoxItem recentFilePrefab;
        public UIListBox srRecentFiles;
        public GameObject goFileInput;
        public Button buCancel;
        public Button buLoad;
        public SettingsStore store;

        // Use this for initialization
        void Start()
        {
            AttachBehavior2FileInput();
            AttachBehavior2RecentFiles();
            AttachBehavior2Cancel();
            AttachBehavior2Load();
        }

        void AttachBehavior2FileInput()
        {
            var inputField = goFileInput.GetComponentInChildren<InputField>();
            inputField.ObserveEveryValueChanged(o => o.text).Do(t => buLoad.enabled = File.Exists(t)).Subscribe();
        }

        void AttachBehavior2RecentFiles()
        {
            var files = store.RecentFiles;
            foreach (var recentFile in store.RecentFiles)
            {
                recentFilePrefab.Path = recentFile.Path;
                recentFilePrefab.DateTime = recentFile.Time;
                srRecentFiles.Add(recentFilePrefab);
            }
            if (store.RecentFiles.Count > 0)
            {
                FileModeSettings.Current = store.RecentFiles.First();
            }
            else
            {
                FileModeSettings.Current = new FileModeSettings();
            }
            srRecentFiles.OnSelectionChanged += RecentFileChanged;
        }

        private void RecentFileChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            FileModeSettings.Current = store.RecentFiles[e.index];
            var browseField = goFileInput.GetComponentInChildren<InputField>();
            browseField.text = FileModeSettings.Current.Path;
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            scalingField.text = FileModeSettings.Current.Scaling.ToString();
            
        }

        void AttachBehavior2Cancel()
        {
            buCancel.OnClickAsObservable()
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }

        void AttachBehavior2Load()
        {
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            var pathField = GameObject.Find("Path field").GetComponent<InputField>();
            buLoad.OnClickAsObservable()
                .Select(_ => pathField.text == FileModeSettings.Current.Path ? FileModeSettings.Current : new FileModeSettings())
                .Do(fms => FileModeSettings.Current = fms)
                .Do(_ => FileModeSettings.Current.Scaling = scalingField.text.Length == 0 ? 1.0f : float.Parse(scalingField.text))
                .Do(_ => FileModeSettings.Current.Path = pathField.text)
                .Do(_ => FileModeSettings.Current.Time = DateTime.Now)
                .Do(_ => store.Add(FileModeSettings.Current))
                .Do(_ => store.Serialize())
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }
    }
}