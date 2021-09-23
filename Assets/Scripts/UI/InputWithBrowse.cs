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
        public enum DialogType
        {
            Save,
            Load,
        }
        
        [SerializeField] private Button BrowseButton;
        [SerializeField] private TMP_InputField PathField;

        public bool FolderMode = false;
        public string InitialPath = null;
        public string Title = "Load";
        public string ButtonText = "Select";
        public string[] Filters;
        public DialogType TypeOfDialog = DialogType.Load;

        public string FilePath => PathField.text;

        private void Start()
        {
            BrowseButton.OnClickAsObservable().Subscribe(_ => Browse());
        }

        private void Browse()
        {
            FileBrowser.SetFilters(true, Filters);
            if (TypeOfDialog == DialogType.Load)
            {
                FileBrowser.ShowLoadDialog(path => PathField.text = path[0],
                                           () => { }, FolderMode, false, GetInitialPath(),
                                           Title, ButtonText);
            }
            else
            {
                FileBrowser.ShowSaveDialog(path => PathField.text = path[0],
                                           () => { }, FolderMode, false, GetInitialPath(),
                                           Title, ButtonText);
            }
        }

        private string GetInitialPath()
        {
            if (!string.IsNullOrWhiteSpace(PathField.text)) return Path.GetDirectoryName(PathField.text);
            if (!string.IsNullOrWhiteSpace(InitialPath)) return InitialPath;
            return null;
        }
    }
}