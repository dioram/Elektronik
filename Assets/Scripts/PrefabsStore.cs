using System;
using UnityEngine;

namespace Elektronik
{
    /// <summary> Class for easy access to some popular prefabs. </summary>
    internal class PrefabsStore : MonoBehaviour
    {
        public Texture2D DefaultCursor;
        public Texture2D LeftRightCursor;
        public Texture2D NorthEastCursor;
        public Texture2D NorthWestCursor;
        public Texture2D TopDownCursor;
        public GameObject TooltipPrefab;

        public static PrefabsStore Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else throw new Exception("There can't be more than one PrefabsStore in scene.");
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}