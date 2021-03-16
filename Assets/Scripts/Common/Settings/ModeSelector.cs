using System;
using UnityEngine;

namespace Elektronik.Common.Settings
{
    public class ModeSelector : MonoBehaviour
    {
        public GameObject filePlayer;
        public GameObject onlinePlayer;

        private void Start()
        {
            switch (SettingsBag.Mode)
            {
                case Mode.Offline:
                    {
                        filePlayer.SetActive(true);
                        break;
                    }
                case Mode.Online:
                    {
                        onlinePlayer.SetActive(true);
                        break;
                    }
                default:
                    throw new NullReferenceException("No modes are initialized");
            }
        }
    }
}
