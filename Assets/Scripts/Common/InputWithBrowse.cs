using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using SimpleFileBrowser;
using UniRx;

namespace Elektronik.Common
{
    public class InputWithBrowse : MonoBehaviour
    {
        private Button m_buBrowse;

        [HideInInspector]
        public InputField ifFilePath;

        public bool folderMode = false;
        public string initialPath = null;
        public string title = "Load";
        public string buttonText = "Select";
        public bool showAllFiles = false;
        public string[] filters;


        // Use this for initialization
        void Start()
        {
            m_buBrowse = GetComponentInChildren<Button>();
            m_buBrowse.OnClickAsObservable().Subscribe(_ => Browse());

            ifFilePath = GetComponentInChildren<InputField>();

            FileBrowser.SetFilters(showAllFiles, filters);
        }

        void Browse()
        {
            FileBrowser.ShowLoadDialog((path) => ifFilePath.text = path, () => { }, folderMode, initialPath, title, buttonText);
        }
    }
}