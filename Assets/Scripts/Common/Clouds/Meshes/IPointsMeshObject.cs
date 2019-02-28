using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    public interface IPointsMeshObject
    {
        int MaxPointsCount { get; }
        void Get(int idx, out Vector3 tetrahedronCG, out Color color);
        void Set(int idx, Matrix4x4 offset, Color color);
        void Set(int idx, Color color);
        void Set(int idx, Matrix4x4 offset);
        bool Exists(int idx);
        void Repaint();
        void GetAll(out Vector3[] tetrahedronsCGs, out Color[] colors);
        void Clear();
    }
}
