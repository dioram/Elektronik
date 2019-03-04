using System;
using UnityEngine;

namespace Elektronik.Common.Data
{
    public struct SlamPoint
    {
        public Vector3 position;
        public bool justColored;
        public int id;
        public Color defaultColor;
        public Color color;
        public bool isRemoved;
        public bool isNew;
        public string message;
    }
}
