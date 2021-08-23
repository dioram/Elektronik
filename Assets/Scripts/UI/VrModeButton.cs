using UnityEngine;
using UnityEngine.XR.Management;

namespace Elektronik.UI
{
    public class VrModeButton : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(XRGeneralSettings.Instance.Manager.activeLoaders.Count > 0);
        }
    }
}