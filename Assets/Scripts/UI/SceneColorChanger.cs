﻿using UnityEngine;
using HSVPicker;

namespace Elektronik.UI
{
    public class SceneColorChanger : MonoBehaviour
    {
        public ColorPicker Picker;
        public Camera[] Cameras;

        private void Start()
        {
            Picker.CurrentColor = Camera.main.backgroundColor;
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