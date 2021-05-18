using System.IO;
using SimpleFileBrowser;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class InputWithBrowse : MonoBehaviour
    {
        private Button _browseButton;
        private InputField _ifFilePath;

        public bool FolderMode = false;
        public string InitialPath = null;
        public string Title = "Load";
        public string ButtonText = "Select";
        public string[] Filters;

        void Start()
        {
            _browseButton = GetComponentInChildren<Button>();
            _browseButton.OnClickAsObservable().Subscribe(_ => Browse());
            _ifFilePath = GetComponentInChildren<InputField>();
        }

        void Browse()
        {
            FileBrowser.SetFilters(true, Filters);
            FileBrowser.ShowLoadDialog(path => _ifFilePath.text = path[0],
                                       () => { },
                                       FolderMode,
                                       false,
                                       GetInitialPath(),
                                       Title,
                                       ButtonText);
        }

        private string GetInitialPath()
        {
            if (!string.IsNullOrWhiteSpace(_ifFilePath.text)) return Path.GetDirectoryName(_ifFilePath.text);
            if (!string.IsNullOrWhiteSpace(InitialPath)) return InitialPath;
            return null;
        }
    }
}