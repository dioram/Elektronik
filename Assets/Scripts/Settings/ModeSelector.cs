// ReSharper disable once RedundantUsingDirective
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Settings
{
    public class ModeSelector : MonoBehaviour
    {
        public static Mode Mode = Mode.Invalid;
    
        public GameObject FilePlayer;
        public GameObject OnlinePlayer;
        public Text OnlineLabel;
        public Text OfflineLabel;

        private void Awake()
        {
            switch (Mode)
            {
                case Mode.Offline:
                    InitOffline();
                    break;
                case Mode.Online:
                    InitOnline();
                    break;
                case Mode.Invalid:
#if UNITY_EDITOR
                    Mode = Mode.Online;
                    InitOnline();
                    break;
#else
                    throw new NullReferenceException("No modes are initialized");
#endif
            }
        }

        private void InitOffline()
        {
            if (FilePlayer != null) FilePlayer.SetActive(true);
            if (OnlineLabel != null) OnlineLabel.enabled = false;
            if (OfflineLabel != null) OfflineLabel.enabled = true;
        }

        private void InitOnline()
        {
            if (OnlinePlayer != null) OnlinePlayer.SetActive(true);
            if (OnlineLabel != null) OnlineLabel.enabled = true;
            if (OfflineLabel != null) OfflineLabel.enabled = false;
        }
    }
}
