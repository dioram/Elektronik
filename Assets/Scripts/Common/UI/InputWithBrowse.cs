using SimpleFileBrowser;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
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
            FileBrowser.ShowLoadDialog(
                path => ifFilePath.text = path[0], 
                () => { }, 
                initialPath: initialPath, 
                title: title, 
                loadButtonText: buttonText);
        }
    }
}