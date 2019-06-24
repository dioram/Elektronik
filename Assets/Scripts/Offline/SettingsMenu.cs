using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using Elektronik.Common;
using System.IO;
using Elektronik.Common.UI;

namespace Elektronik.Offline
{
    public class SettingsMenu : MonoBehaviour
    {
        const string SETTINGS_FILE = @"offline\settings.dat";

        public UIListBox lbRecentFiles;
        public GameObject goFileInput;
        public Button buCancel;
        public Button buLoad;
        public SettingsBagStore store;

        void Awake()
        {
            SettingsBag.Mode = Mode.Offline;
        }

        // Use this for initialization
        void Start()
        {
            store.Deserialize(SETTINGS_FILE);
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
            var files = store.Recent;
            foreach (var recentFile in store.Recent)
            {
                var recentFileItem = lbRecentFiles.Add() as RecentFileListBoxItem;
                recentFileItem.Path = recentFile[SettingName.Path].As<string>();
                recentFileItem.DateTime = recentFile.ModificationTime;
            }
            if (store.Recent.Count > 0)
            {
                SettingsBag.Current = store.Recent.First();
            }
            else
            {
                SettingsBag.Current = new SettingsBag();
            }
            lbRecentFiles.OnSelectionChanged += RecentFileChanged;
        }

        private void RecentFileChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            SettingsBag.Current = store.Recent[e.index];
            var browseField = goFileInput.GetComponentInChildren<InputField>();
            browseField.text = SettingsBag.Current[SettingName.Path].As<string>();
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            scalingField.text = SettingsBag.Current[SettingName.Scale].As<float>().ToString();
        }

        void AttachBehavior2Cancel()
        {
            buCancel.OnClickAsObservable()
                .Do(_ => SettingsBag.Current = null)
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Main menu", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }

        void AttachBehavior2Load()
        {
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            var pathField = GameObject.Find("Path field").GetComponent<InputField>();
            buLoad.OnClickAsObservable()
                .Select(_ =>
                {
                    if (SettingsBag.Current.TryGetValue(SettingName.Path, out Setting pathSetting))
                    {
                        return pathField.text == SettingsBag.Current[SettingName.Path].As<string>() ? SettingsBag.Current : new SettingsBag();
                    }
                    return new SettingsBag();
                })
                .Do(fms => SettingsBag.Current = fms)
                .Do(_ => SettingsBag.Current.Change(SettingName.Scale, scalingField.text.Length == 0 ? 1.0f : float.Parse(scalingField.text)))
                .Do(_ => SettingsBag.Current.Change(SettingName.Path, pathField.text))
                .Do(_ => store.Add(SettingsBag.Current))
                .Do(_ => store.Serialize(SETTINGS_FILE))
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }
    }
}