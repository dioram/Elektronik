using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elektronik.Common.UI
{
    public class MainMenuButtons : MonoBehaviour
    {
        public void OnExitClick()
        {
            Application.Quit();
        }

        public void OnOfflineModeClick()
        {
            SceneManager.LoadScene("Offline settings", LoadSceneMode.Single);
        }

        public void OnOnlineModeClick()
        {
            SceneManager.LoadScene("Online settings", LoadSceneMode.Single);
        }
    }
}

