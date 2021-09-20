using System;
using UnityEngine;

namespace Elektronik.Settings
{
    [Serializable]
    public class SceneSettingsBag : SettingsBag
    {
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
        
        [CheckForEquals] public float PointSize;
        [CheckForEquals] public int Duration;
        [CheckForEquals] public int GridState;
        [CheckForEquals] public int AxisState;
        [CheckForEquals] public SerialisedColor SceneColor = new SerialisedColor(0.8f, 0.8f, 0.8f, 1);
    }
}