using Elektronik.Settings;
using Elektronik.UI.Localization;
using UnityEngine;
using UnityEngine.XR.Management;

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
            public readonly Mode SceneMode;
            public readonly bool VrOnly;

            public Row(string name, Mode mode = Mode.All, bool vrOnly = false)
            {
                IsLabel = true;
                Name = name;
                Main = "";
                Alternative = "";
                SceneMode = mode;
                VrOnly = vrOnly;
            }

            public Row(string name, string main, Mode mode = Mode.All, bool vrOnly = false)
            {
                IsLabel = false;
                Name = name;
                Main = main;
                Alternative = "";
                SceneMode = mode;
                VrOnly = vrOnly;
            }

            public Row(string name, string main, string alternative, Mode mode = Mode.All, bool vrOnly = false)
            {
                IsLabel = false;
                Name = name;
                Main = main;
                Alternative = alternative;
                SceneMode = mode;
                VrOnly = vrOnly;
            }

            public enum Mode
            {
                All,
                Online,
                Offline,
            }

            public bool IsAvailable(Settings.Mode mode) =>
                    SceneMode switch
                    {
                        _ when VrOnly && !VrController.IsVrEnabled => false,
                        
                        Mode.All => true,
                        Mode.Online when mode == Settings.Mode.Online => true,
                        Mode.Offline when mode == Settings.Mode.Offline => true,
                        _ when mode == Settings.Mode.Invalid => true,
                        _ => false
                    };
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
            new Row ("Play/Pause", "Space", Row.Mode.Offline),
            new Row ("Stop", "Ctrl+Delete", Row.Mode.Offline),
            new Row ("Rewind forward", "[", Row.Mode.Offline),
            new Row ("Rewind backward", "]", Row.Mode.Offline),
            new Row ("One frame forward", ".", Row.Mode.Offline),
            new Row ("One frame backward", ",", Row.Mode.Offline),
            new Row ("Take snapshot", "Ctrl+T"),
            new Row ("Load snapshot", "Ctrl+O"),
            new Row ("Clear map", "Shift+Delete", Row.Mode.Online),
            new Row ("Start/Stop recording", "Ctrl+R"),
            new Row ("Show scene tools", "Ctrl+Y"),
            new Row ("Show source tree", "Ctrl+F"),
            new Row ("Show analytics tools", "Ctrl+U"),
            new Row ("Toggle grid", "Ctrl+G"),
            new Row ("Toggle axis", "Ctrl+H"),
            new Row ("Toggle mesh color", "Ctrl+M"),
            new Row ("Go to VR mode", "F2", vrOnly: true),
            new Row ("VR help", "F3", vrOnly: true),
        };

        private void Start()
        {
            foreach (var control in _controls)
            {
                if (!control.IsAvailable(ModeSelector.Mode)) continue;
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