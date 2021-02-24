using Elektronik.Common.Settings;
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
            ModeSelector.Mode = Mode.Offline;
            SceneManager.LoadScene("Scenes/Settings", LoadSceneMode.Single);
        }

        public void OnOnlineModeClick()
        {
            ModeSelector.Mode = Mode.Online;
            SceneManager.LoadScene("Scenes/Settings", LoadSceneMode.Single);
        }
    }
}

