using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Elektronik.Common.Settings;

namespace Elektronik.Offline.UI
{
    public class Return2OfflineMenu : MonoBehaviour
    {
        private Button _button;
        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        void Start()
        {
            _button.onClick.AddListener(OnBackToMenuClick);
        }

        void OnBackToMenuClick()
        {
            switch (SettingsBag.Mode)
            {
                case Mode.Offline:
                    SceneManager.LoadScene(@"Assets/Scenes/Offline/Offline settings.unity", LoadSceneMode.Single);
                    break;
                case Mode.Online:
                    SceneManager.LoadScene(@"Assets/Scenes/Online/Online settings.unity", LoadSceneMode.Single);
                    break;
                default:
                    throw new Exception("Go to this scene from settings scene");
            }
        }

    }
}