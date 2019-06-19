using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.Common.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        Button m_bOnlineMode;
        Button m_bOfflineMode;
        Button m_bExit;

        private void Start()
        {
            m_bOnlineMode = GameObject.Find("Online mode").GetComponent<Button>();
            m_bOnlineMode.onClick.AddListener(Move2OnlineSettingsScene);

            m_bOfflineMode = GameObject.Find("Offline mode").GetComponent<Button>();
            m_bOfflineMode.onClick.AddListener(Move2OfflineSettingsScene);

            m_bExit = GameObject.Find("Exit").GetComponent<Button>();
            m_bExit.onClick.AddListener(Exit);
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

