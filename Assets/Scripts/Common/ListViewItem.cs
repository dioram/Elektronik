using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace Elektronik.Common
{
    public class ListViewItem : MonoBehaviour
    {
        public void SetText(string text)
        {
            GetComponentInChildren<Text>().text = text;
        }
    }
}