using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    public interface IPointsMeshData
    {
        CloudPoint Get(int idx);
        void Set(CloudPoint point);
        void Set(IEnumerable<CloudPoint> points);
        void Set(int idx, Color color);
        void Set(int idx, Vector3 offset);
        bool Exists(int idx);
        IEnumerable<CloudPoint> GetAll();
        void Clear();
    }
}
