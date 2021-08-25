using System;
using UnityEngine;

namespace Elektronik.UI.Windows
{
    public class ImageStore : MonoBehaviour
    {
        public Texture2D DefaultCursor;
        public Texture2D LeftRightCursor;
        public Texture2D NorthEastCursor;
        public Texture2D NorthWestCursor;
        public Texture2D TopDownCursor;

        public static ImageStore Instance;

        private void Start()
        {
            if (Instance == null) Instance = this;
            else throw new Exception("There can't be more than one ImageStore in scene.");
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}