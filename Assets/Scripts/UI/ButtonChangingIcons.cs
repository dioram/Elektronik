using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    public class ButtonChangingIcons : ChangingButton
    {
        public Sprite[] Icons;
        public Image TargetImage;

        protected override void Start()
        {
            MaxState = Icons.Length;
            base.Start();
        }

        protected override void SetValue()
        {
            TargetImage.sprite = Icons[State];
        }
    }
}