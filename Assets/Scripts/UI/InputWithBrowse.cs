using System.IO;
using SimpleFileBrowser;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class InputWithBrowse : MonoBehaviour
    {
        [SerializeField] private Button BrowseButton;
        [SerializeField] private TMP_InputField PathField;

        public bool FolderMode = false;
        public string InitialPath = null;
        public string Title = "Load";
        public string ButtonText = "Select";
        public string[] Filters;

        private void Start()
        {
            BrowseButton.OnClickAsObservable().Subscribe(_ => Browse());
        }

        private void Browse()
        {
            FileBrowser.SetFilters(true, Filters);
            FileBrowser.ShowLoadDialog(path => PathField.text = path[0],
                                       () => { },
                                       FolderMode,
                                       false,
                                       GetInitialPath(),
                                       Title,
                                       ButtonText);
        }

        private string GetInitialPath()
        {
            if (!string.IsNullOrWhiteSpace(PathField.text)) return Path.GetDirectoryName(PathField.text);
            if (!string.IsNullOrWhiteSpace(InitialPath)) return InitialPath;
            return null;
        }
    }
}