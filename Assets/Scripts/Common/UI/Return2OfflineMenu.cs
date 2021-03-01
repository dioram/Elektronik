using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Elektronik.Common.UI
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