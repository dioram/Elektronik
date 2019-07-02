﻿using UnityEngine;
using VRTK;

namespace Elektronik.Common
{
    public class VRTKSetupInitializer : MonoBehaviour
    {
        public VRTK_SDKSetup setup;
        public VRTKManagerInitializer m_managerInitializer;
        public Transform VRMode;

        private void Start()
        {
            m_managerInitializer.Setup = setup;
            transform.parent = VRMode;
        }
    }
}