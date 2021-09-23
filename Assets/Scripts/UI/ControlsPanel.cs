using Elektronik.UI.Localization;
using UnityEngine;

namespace Elektronik.UI
{
    public class ControlsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject RowPrefab;

        struct Row
        {
            public readonly bool IsLabel;
            public readonly string Name;
            public readonly string Main;
            public readonly string Alternative;

            public Row(string name, string main = "", string alternative = "")
            {
                IsLabel = false;
                Name = name;
                Main = main;
                Alternative = alternative;
            }
        }

        private readonly Row[] _controls =
        {
            new Row ("Camera controls"),
            new Row ("X", "A/D"),
            new Row ("Y", "Q/E"),
            new Row ("Z", "W/S", "Mouse wheel"),
            new Row ("Forward", "LMB + RMB"),
            new Row ("Yaw", "Left/Right", "Mouse X"),
            new Row ("Pitch", "Up/Down", "Mouse Y"),
            new Row ("Reset", "Backspace"),
            new Row ("Hot keys"),
            new Row ("Help", "F1"),
            new Row ("Play/Pause", "Space"),
            new Row ("Stop", "Ctrl+Delete"),
            new Row ("Rewind forward", "["),
            new Row ("Rewind backward", "]"),
            new Row ("One frame forward", "."),
            new Row ("One frame backward", ","),
            new Row ("Take snapshot", "Ctrl+T"),
            new Row ("Load snapshot", "Ctrl+O"),
            new Row ("Clear map", "Shift+Delete"),
            new Row ("Start/Stop recording", "Ctrl+R"),
            new Row ("Show scene tools", "Ctrl+Y"),
            new Row ("Show source tree", "Ctrl+F"),
            new Row ("Show analytics tools", "Ctrl+U"),
            new Row ("Toggle grid", "Ctrl+G"),
            new Row ("Toggle axis", "Ctrl+H"),
            new Row ("Toggle mesh color", "Ctrl+M"),
            new Row ("Go to VR mode", "F2"),
            new Row ("VR help", "F3"),
        };

        private void Start()
        {
            foreach (var control in _controls)
            {
                var row = Instantiate(RowPrefab, transform).GetComponent<ControlsRow>();
                if (row is null) continue;

                row.NameLabel.SetLocalizedText(control.Name);
                if (control.IsLabel)
                {
                    row.MainInput.gameObject.SetActive(false);
                    row.AltInput.gameObject.SetActive(false);
                    row.Layout.enabled = true;
                }
                row.MainInput.text = control.Main;
                row.AltInput.text = control.Alternative;
            }
        }
    }
}