using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.ListBox
{
    public class ListBoxItem : MonoBehaviour
    {
        public event EventHandler OnClick;

        protected Button ClickButton;

        protected virtual void Awake()
        {
            ClickButton = GetComponent<Button>();
        }

        protected virtual void Start()
        {
            if (ClickButton == null) return;
            ClickButton.onClick.AddListener(() => OnClick?.Invoke(this, null));
        }
    }
}