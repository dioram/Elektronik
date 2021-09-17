using UnityEngine;
using UnityEngine.XR.Management;

namespace Elektronik
{
    public class VrController : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private GameObject VrModeButton;
        [SerializeField] private GameObject VrModeGameObject;
        [SerializeField] private GameObject NonVrModeGameObject;
        [SerializeField] private GameObject VrHelpMenu;

        #endregion
        
        public bool IsInVrMode { get; private set; } = false;

        public static bool IsVrEnabled
        {
            get
            {
                if (XRGeneralSettings.Instance is null) return false;
                return XRGeneralSettings.Instance.Manager.activeLoaders.Count > 0;
            }
        }

        private void Start()
        {
            VrModeButton.SetActive(IsVrEnabled);
        }

        public void ToggleVrMode()
        {
            VrModeGameObject.SetActive(!VrModeGameObject.activeSelf);
            NonVrModeGameObject.SetActive(!VrModeGameObject.activeSelf);
            IsInVrMode = !IsInVrMode;
        }

        public void ToggleVrHelpMenu()
        {
            VrHelpMenu.SetActive(!VrHelpMenu.activeSelf);
        }
    }
}