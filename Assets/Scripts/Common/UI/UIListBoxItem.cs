using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    [RequireComponent(typeof(Button))]
    public class UIListBoxItem : MonoBehaviour
    {
        public event EventHandler OnClick;

        protected Button ClickButton;

        protected virtual void Awake()
        {
            ClickButton = GetComponent<Button>();
        }

        protected virtual void Start()
        {
            ClickButton.onClick.AddListener(() => OnClick?.Invoke(this, null));
        }
    }
}