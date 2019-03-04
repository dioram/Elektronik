﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Elektronik.Common
{
    public class VRTKSetupInitializer : MonoBehaviour
    {
        public VRTK_SDKSetup setup;
        public VRTKManagerInitializer m_managerInitializer;

        private void Start()
        {
            m_managerInitializer.Setup = setup;
        }
    }
}