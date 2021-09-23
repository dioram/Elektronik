using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.Buttons
{
    public class ButtonChangingIcons : ChangingButton
    {
        public Sprite[] Icons;
        public Image TargetImage;

        protected override void Awake()
        {
            base.Awake();
            MaxState = Icons.Length;
        }

        protected override void SetValue()
        {
            TargetImage.sprite = Icons[State];
        }
    }
}