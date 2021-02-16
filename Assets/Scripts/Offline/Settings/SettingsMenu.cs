using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Linq;
using Elektronik.Common.Settings;
using Elektronik.Common.UI;
using System.IO;
using Elektronik.Offline.UI;

namespace Elektronik.Offline.Settings
{
    public class SettingsMenu : MonoBehaviour
    {
        const string SETTINGS_FILE = @"offline\settings.dat";

        public UIListBox lbRecentFiles;
        public UIListBox lbRecentImages;
        public Button buCancel;
        public Button buLoad;
        public SettingsBagStore store;
        public InputField filePathField;
        public InputField imagePathField;

        void Awake()
        {
            SettingsBag.Mode = Mode.Offline;
        }

        // Use this for initialization
        void Start()
        {
            store.Deserialize(SETTINGS_FILE);
            AttachBehavior2FileInput();
            AttachBehavior2Cancel();
            AttachBehavior2Load();
            AttachBehavior2RecentFiles();
        }

        void AttachBehavior2FileInput()
        {
            filePathField
                .ObserveEveryValueChanged(o => o.text)
                .Do(t => buLoad.enabled = File.Exists(t))
                .Subscribe();
        }

        void AttachBehavior2RecentFiles()
        {
            foreach (var recentFile in store.Recent)
            {
                var recentFileItem = lbRecentFiles.Add() as RecentFileListBoxItem;
                var recentImageItem = lbRecentImages.Add() as RecentFileListBoxItem;
                if (recentFile.TryGetValue(SettingName.FilePath, out Setting fileSetting))
                {
                    recentFileItem.Path = fileSetting.As<string>();
                    recentFileItem.DateTime = recentFile.ModificationTime;
                }
                if (recentFile.TryGetValue(SettingName.ImagePath, out Setting imageSetting))
                {
                    recentImageItem.Path = imageSetting.As<string>();
                    recentImageItem.DateTime = recentFile.ModificationTime;
                }
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
            lbRecentImages.OnSelectionChanged += RecentImageChanged;
        }

        private void RecentFileChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            SettingsBag.Current = store.Recent[e.index];
            filePathField.text = SettingsBag.Current[SettingName.FilePath].As<string>();
            var scalingField = GameObject.Find("Input scaling").GetComponent<InputField>();
            scalingField.text = SettingsBag.Current[SettingName.Scale].As<float>().ToString(CultureInfo.CurrentCulture);
        }

        private void RecentImageChanged(object sender, UIListBox.SelectionChangedEventArgs e)
        {
            SettingsBag.Current = store.Recent[e.index];
            imagePathField.text = SettingsBag.Current[SettingName.ImagePath].As<string>();
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
            buLoad.OnClickAsObservable()
                .Select(_ =>
                {
                    if (SettingsBag.Current.TryGetValue(SettingName.FilePath, out Setting _))
                    {
                        var currentFilePath = SettingsBag.Current[SettingName.FilePath].As<string>();
                        var currentImagePath = SettingsBag.Current[SettingName.ImagePath].As<string>();
                        return filePathField.text == currentFilePath && imagePathField.text == currentImagePath ? SettingsBag.Current : new SettingsBag();
                    }
                    return new SettingsBag();
                })
                .Do(fms => SettingsBag.Current = fms)
                .Do(_ => SettingsBag.Current.Change(SettingName.Scale, scalingField.text.Length == 0 ? 1.0f : float.Parse(scalingField.text)))
                .Do(_ => SettingsBag.Current.Change(SettingName.FilePath, filePathField.text))
                .Do(_ => SettingsBag.Current.Change(SettingName.ImagePath, imagePathField.text))
                .Do(_ => store.Add(SettingsBag.Current))
                .Do(_ => store.Serialize(SETTINGS_FILE))
                .Do(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single))
                .Subscribe();
        }
    }
}