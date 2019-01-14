using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common
{
    [RequireComponent(typeof(Button))]
    public class UIListBoxItem : MonoBehaviour
    {
        public event EventHandler OnClick;

        private Button m_button;
        private void Start()
        {
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(() => { if (OnClick != null) OnClick(this, null); });
        }
    }
}