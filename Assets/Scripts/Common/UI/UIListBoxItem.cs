using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    [RequireComponent(typeof(Button))]
    public class UIListBoxItem : MonoBehaviour
    {
        public event EventHandler OnClick;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(() => { if (OnClick != null) OnClick(this, null); });
        }


    }
}