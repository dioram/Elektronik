using Elektronik.PluginsSystem.UnitySide;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Button))]
    public class Return2OfflineMenu : MonoBehaviour
    {
        public PluginsPlayer Player;
        
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
            Player.ClearMap();
            SceneManager.LoadScene("Scenes/Settings", LoadSceneMode.Single);
        }
    }
}