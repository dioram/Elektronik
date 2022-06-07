using System;
using UnityEngine;

namespace Elektronik.Settings
{
    /// <summary> Settings for scene. </summary>
    [Serializable]
    public class SceneSettingsBag : SettingsBag
    {
        // TODO: Check why it can't be just color, and write explanation here.
        [Serializable]
        public class SerialisedColor
        {
            public float R, G, B, A;

            public SerialisedColor(float r, float g, float b, float a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }

            public static implicit operator Color(SerialisedColor c) => new Color(c.R, c.G, c.B, c.A);
            public static implicit operator SerialisedColor(Color c) => new SerialisedColor(c.r, c.g, c.b, c.a);
        }

        /// <summary> Render size of points, in meters. </summary>
        [Tooltip("Render size of points, in meters.")] [CheckForEquals]
        public float PointSize;
        // TODO: Add here size of observations.

        /// <summary> Time of rendering traces for updated objects. </summary>
        [Tooltip("Time of rendering traces for updated objects.")] [CheckForEquals]
        public int Duration;

        // TODO: Use enum.
        /// <summary> Grid rendering state. </summary>
        [CheckForEquals] public int GridState;

        // TODO: Use bool.
        /// <summary> Axis rendering state. </summary>
        [CheckForEquals] public int AxisState;

        /// <summary> Scene background color. </summary>
        [Tooltip("Scene background color.")] [CheckForEquals]
        public SerialisedColor SceneColor = new SerialisedColor(0.8f, 0.8f, 0.8f, 1);
    }
}