using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Data.PackageObjects
{
    public struct SlamTrackedObject
    {
        public int id;
        public Color color;
        public Vector3 position;
        public Quaternion rotation;

        public SlamTrackedObject(int id, Color color = default, Vector3 position = default, Quaternion rotation = default)
        {
            this.id = id;
            this.color = color;
            this.position = position;
            this.rotation = rotation;
        }
    }
}
