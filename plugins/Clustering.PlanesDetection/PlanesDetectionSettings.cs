using System;
using Elektronik.Clustering.PlanesDetection.Native;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.Clustering.PlanesDetection
{
    [Serializable]
    public class PlanesDetectionSettings : SettingsBag
    {
        public int DepthThreshold = 100;
        public float Epsilon = 0.05f;
        public int NumberOfStartPoints = 10;
        public int NumberOfPoints = 30;
        public int NumberOfSteps = 10;
        public float CountRatio = 0.05f;
        public float DeltaAngle = 15;

        [Tooltip("Show only vertical and horizontal planes")]
        public bool UseGravity = true;

        public Vector3 GravityVector = new Vector3(0, 0, -9.8f);
        public float GravityDeltaAngle = 5;

        internal Preferences ToPrefs()
        {
            return new Preferences(DepthThreshold, Epsilon, NumberOfStartPoints, NumberOfPoints, NumberOfSteps,
                                   CountRatio, Mathf.Cos(Mathf.Deg2Rad * DeltaAngle),
                                   new Vector3d(GravityVector.x, GravityVector.y, GravityVector.z), UseGravity,
                                   Mathf.Cos(Mathf.Deg2Rad * GravityDeltaAngle));
        }
    }
}