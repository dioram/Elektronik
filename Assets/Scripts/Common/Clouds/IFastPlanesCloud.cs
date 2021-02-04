using System.Collections.Generic;
using Elektronik.Common.Clouds.Meshes;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public interface IFastPlanesCloud
    {
        void Clear();
        bool Exists(int idx);
        CloudPlane Get(int idx);
        IEnumerable<CloudPlane> GetAll();
        void Set(CloudPlane plane);
        void Set(int idx, Color color);
        void Set(int idx, Vector3 offset, Vector3 normal);
        void Set(IEnumerable<CloudPlane> planes);
        void SetActive(bool value);
    }
}