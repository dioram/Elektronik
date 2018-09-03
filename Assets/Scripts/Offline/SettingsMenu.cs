using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SimpleFileBrowser;
using UnityEditor;
using System.Linq;
using Elektronik.Common;

namespace Elektronik.Offline
{
    public class SettingsMenu : MonoBehaviour
    {
        public ScrollRect srRecentFiles;
        public GameObject goFileInput;
        public Button buCancel;
        public Button buLoad;

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
        }

        void AttachBehavior2RecentFiles()
        {
            // TODO: добавить сериализацию
        }

        void AttachBehavior2Cancel()
        {

        }

        void AttachBehavior2Load()
        {
            buLoad.OnClickAsObservable().Subscribe(_ => UnityEngine.SceneManagement.SceneManager.LoadScene("Empty", UnityEngine.SceneManagement.LoadSceneMode.Single));
        }
    }
}