using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class VersionLabel : MonoBehaviour
    {
        private Text _label;
        private void Start()
        {
            _label = GetComponent<Text>();
            _label.text = $"Version {Application.version}";
        }
    }
}