using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamPoint
    {
        public Vector3 position;
        public int id;
        public Color defaultColor;
        public Color color;
        public string message;
        public override string ToString()
        {
            return message ?? "SlamPoint";
        }
    }
}
