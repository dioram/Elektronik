using System;
using Elektronik.PluginsSystem.UnitySide;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Button))]
    public class Return2Menu : MonoBehaviour
    {
        public PluginsPlayer Player;

        public void OnBackToSettings()
        {
            if (Player != null)
            {
                try
                {
                    Player.ClearMap();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            SceneManager.LoadScene("Scenes/Settings", LoadSceneMode.Single);
        }

        public void OnBackToMenu()
        {
            SceneManager.LoadScene("Scenes/Main menu", LoadSceneMode.Single);
        }
    }
}