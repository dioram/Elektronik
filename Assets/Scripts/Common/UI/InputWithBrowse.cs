using SimpleFileBrowser;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    public class InputWithBrowse : MonoBehaviour
    {
        private Button _browseButton;
        private InputField _ifFilePath;

        public bool folderMode = false;
        public string initialPath = null;
        public string title = "Load";
        public string buttonText = "Select";
        public string[] filters;


        // Use this for initialization
        void Start()
        {
            _browseButton = GetComponentInChildren<Button>();
            _browseButton.OnClickAsObservable().Subscribe(_ => Browse());
            _ifFilePath = GetComponentInChildren<InputField>();
        }

        void Browse()
        {
            FileBrowser.ShowLoadDialog(path => _ifFilePath.text = path[0],
                                       () => { },
                                       folderMode,
                                       false,
                                       initialPath,
                                       title,
                                       buttonText);
        }
    }
}