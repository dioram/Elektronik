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
            SceneManager.LoadScene("Scenes/Settings", LoadSceneMode.Single);
        }
    }
}