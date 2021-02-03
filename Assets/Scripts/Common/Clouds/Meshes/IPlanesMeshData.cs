using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Clouds.Meshes
{
    public interface IPlanesMeshData
    {
        CloudPlane Get(int idx);
        void Set(CloudPlane plane);
        void Set(IEnumerable<CloudPlane> planes);
        void Set(int idx, Color color);
        void Set(int idx, Vector3 offset, Vector3 normal);
        bool Exists(int idx);
        IEnumerable<CloudPlane> GetAll();
        void Clear();
    }
}