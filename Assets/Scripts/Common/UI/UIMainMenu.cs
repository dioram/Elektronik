using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        private Button _onlineModeButton;
        private Button _offlineModeButton;
        private Button _exitButton;

        private void Start()
        {
            _onlineModeButton = GameObject.Find("Online mode").GetComponent<Button>();
            _onlineModeButton.onClick.AddListener(Move2OnlineSettingsScene);

            _offlineModeButton = GameObject.Find("Offline mode").GetComponent<Button>();
            _offlineModeButton.onClick.AddListener(Move2OfflineSettingsScene);

            _exitButton = GameObject.Find("Exit").GetComponent<Button>();
            _exitButton.onClick.AddListener(Exit);
        }

        private void Move2OnlineSettingsScene()
        {
            SceneManager.LoadScene("Online settings", LoadSceneMode.Single);
        }

        private void Move2OfflineSettingsScene()
        {
            SceneManager.LoadScene("Offline settings", LoadSceneMode.Single);
        }

        private void Exit()
        {
            Application.Quit(0);
        }
    }
}

