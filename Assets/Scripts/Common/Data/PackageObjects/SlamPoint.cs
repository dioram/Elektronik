using UnityEngine;
using System.Runtime.Serialization;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamPoint
    {
        public int id;
        public Vector3 position;
        public Color color;
        public string message;
        public Color defaultColor;
        public override string ToString()
        {
            return message ?? "SlamPoint";
        }
    }
}
