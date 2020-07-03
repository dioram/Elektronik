using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    public interface ILinesMeshData
    {
        bool Exists(int idx);
        void Get(int idx, out Vector3 position1, out Vector3 position2, out Color color);
        void Set(int idx, Vector3 position1, Vector3 position2, Color color1, Color color2);
        void Set(int idx, Vector3 position1, Vector3 position2, Color color);
        void Set(int idx, Color color1, Color color2);
        void Set(int idx, Color color);
        void Set(int idx, Vector3 position1, Vector3 position2);
        void Clear();
    }
}
