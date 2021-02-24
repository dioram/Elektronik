using System;
using SFB;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    public class InputWithBrowse : MonoBehaviour
    {
        private Button _browseButton;

        [HideInInspector] public InputField ifFilePath;

        public bool folderMode = false;
        public string initialPath = null;
        public string title = "Load";
        public string buttonText = "Select";
        public bool showAllFiles = false;
        public string[] filters;


        // Use this for initialization
        void Start()
        {
            _browseButton = GetComponentInChildren<Button>();
            _browseButton.OnClickAsObservable().Subscribe(_ => Browse());

            ifFilePath = GetComponentInChildren<InputField>();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                ifFilePath.text = args[1];
            }
        }

        void Browse()
        {
            if (folderMode)
            {
                StandaloneFileBrowser.OpenFolderPanelAsync("Open file",
                                                           initialPath,
                                                           false,
                                                           path => ifFilePath.text = path[0]);
            }
            else
            {
                StandaloneFileBrowser.OpenFilePanelAsync("Open file",
                                                         initialPath,
                                                         "",
                                                         false,
                                                         path => ifFilePath.text = path[0]);
            }
        }
    }
}