using UnityEngine;

namespace Elektronik.UI
{
    public class ControlsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject RowPrefab;

        struct Row
        {
            public string Name;
            public string Main;
            public string Alternative;
        }

        private readonly Row[] _controls =
        {
            new Row {Name = "X", Main = "A/D"},
            new Row {Name = "Y", Main = "Q/E"},
            new Row {Name = "Z", Main = "W/S", Alternative = "Mouse wheel"},
            new Row {Name = "Forward", Main = "LMB + RMB"},
            new Row {Name = "Yaw", Main = "Left/Right", Alternative = "Mouse X"},
            new Row {Name = "Pitch", Main = "Up/Down", Alternative = "Mouse Y"},
            new Row {Name = "Reset", Main = "Backspace"},
        };

        private void Start()
        {
            foreach (var control in _controls)
            {
                var row = Instantiate(RowPrefab, transform).GetComponent<ControlsRow>();
                if (row is null) return;
            
                row.NameLabel.text = control.Name;
                row.MainInput.text = control.Main;
                row.AltInput.text = control.Alternative;
            }
        }
    }
}