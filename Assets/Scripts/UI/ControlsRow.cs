using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class ControlsRow : MonoBehaviour
    {
        [SerializeField] public TMP_Text NameLabel;
        [SerializeField] public TMP_InputField MainInput;
        [SerializeField] public TMP_InputField AltInput;
        [SerializeField] public HorizontalLayoutGroup Layout;

        private void Start()
        {
            Layout ??= GetComponent<HorizontalLayoutGroup>();
        }
    }
}