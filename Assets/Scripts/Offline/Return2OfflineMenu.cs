using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;
using VRTK;
using Elektronik.Online;
using System;

namespace Elektronik.Offline
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
            if (FileModeSettings.Current != null)
            {
                FileModeSettings.Current = null;
                SceneManager.LoadScene(@"Assets/Scenes/Offline/Offline settings.unity");
            } else if (OnlineModeSettings.Current != null)
            {
                OnlineModeSettings.Current = null;
                SceneManager.LoadScene(@"Assets/Scenes/Online/Online settings.unity");
            }
            else
            {
                throw new Exception("Go to this scene from settings scene");
            }
        }

    }
}