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

        private RecentFile m_current;

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
            srRecentFiles.OnSelectionChanged += RecentFileChanged;
        }

        private void RecentFileChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            m_current = store.RecentFiles[e.index];
            var browseField = goFileInput.GetComponentInChildren<InputField>();
            browseField.text = m_current.Path;
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            scalingField.text = m_current.Settings.scaling.ToString();
            
        }

        void AttachBehavior2Cancel()
        {

        }

        void AttachBehavior2Load()
        {
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            var pathField = GameObject.Find("Path field").GetComponent<InputField>();
            buLoad.OnClickAsObservable()
                .Do(_ => FileModeSettings.Scaling = scalingField.text.Length == 0 ? 1.0f : float.Parse(scalingField.text))
                .Do(_ => FileModeSettings.Path = pathField.text)
                .Do(_ => 
                {
                    RecentFile rf = null;
                    var settings = new FileSettings()
                    {
                        scaling = FileModeSettings.Scaling,
                    };
                    if (m_current != null && FileModeSettings.Path == m_current.Path)
                        rf = m_current;
                    else
                        rf = new RecentFile(FileModeSettings.Path);
                    rf.Update(settings);
                    store.Add(rf);
                })
                .Do(_ => store.Serialize())
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }
    }
}