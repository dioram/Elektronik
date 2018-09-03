using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace Elektronik.Common
{
    public class ListViewItem : MonoBehaviour
    {
        private string m_fullPath;
        public string FullPath
        {
            get
            {
                return m_fullPath;
            }
            set
            {
                m_fullPath = value;
                Name = Path.GetFileNameWithoutExtension(m_fullPath);
            }
        }
        private string m_name;
        public string Name
        {
            get { return m_name; }
            private set
            {
                m_name = value;
                SetName(m_name);
            }
        }

        void SetName(string name)
        {
            GetComponentInChildren<Text>().text = name;
        }
    }
}