using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline.UI
{
    public class SpecialInformationBanner : MonoBehaviour
    {
        public Text uiTextOfBanner;

        public void SetText(string text)
        {
            uiTextOfBanner.text = text;
        }

        public void Clear()
        {
            uiTextOfBanner.text = "";
        }
    }
}
