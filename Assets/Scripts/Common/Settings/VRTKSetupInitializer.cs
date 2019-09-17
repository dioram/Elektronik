using UnityEngine;
using VRTK;

namespace Elektronik.Common.Settings
{
    public class VRTKSetupInitializer : MonoBehaviour
    {
        public VRTK_SDKSetup setup;
        public VRTKManagerInitializer m_managerInitializer;
        public Transform VRMode;

        private void Awake()
        {
            m_managerInitializer.Setup = setup;
            transform.parent = VRMode;
        }
    }
}