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

        private void Start()
        {
            switch (Mode)
            {
                case Mode.Offline:
                {
                    if (FilePlayer != null) FilePlayer.SetActive(true);
                    if (OnlineLabel != null) OnlineLabel.enabled = false;
                    if (OfflineLabel != null) OfflineLabel.enabled = true;
                    break;
                }
                case Mode.Online:
                {
                    if (OnlinePlayer != null) OnlinePlayer.SetActive(true);
                    if (OnlineLabel != null) OnlineLabel.enabled = true;
                    if (OfflineLabel != null) OfflineLabel.enabled = false;
                    break;
                }
                default:
                    throw new NullReferenceException("No modes are initialized");
            }
        }
    }
}
