using UnityEngine;
using UnityEngine.UI.Extensions.ColorPicker;

namespace Elektronik.UI
{
    public class SceneColorChanger : MonoBehaviour
    {
        public ColorPickerControl Picker;
        public Camera[] Cameras;

        private void Awake()
        {
            Picker.onValueChanged.AddListener(color =>
            {
                foreach (var cam in Cameras)
                {
                    cam.backgroundColor = color;
                }
            });
        }
    }
}