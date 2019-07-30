using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Elektronik.Common.Settings;

namespace Elektronik.Offline.UI
{
    public class Return2OfflineMenu : MonoBehaviour
    {
        Button m_button;
        private void Awake()
        {
            m_button = GetComponent<Button>();
        }

        void Start()
        {
            m_button.onClick.AddListener(OnBackToMenuClick);
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