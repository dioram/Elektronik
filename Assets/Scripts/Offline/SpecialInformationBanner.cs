using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Offline
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
