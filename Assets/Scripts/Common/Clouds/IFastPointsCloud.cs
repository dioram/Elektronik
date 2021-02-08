using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public interface IFastPointsCloud
    {
        void Clear();
        bool Exists(int idx);
        CloudPoint Get(int idx);
        IEnumerable<CloudPoint> GetAll();
        void Set(CloudPoint point);
        void Set(int idx, Color color);
        void Set(int idx, Vector3 translation);
        void Set(IEnumerable<CloudPoint> points);
        void Remove(int idx);
        void Remove(IEnumerable<int> pointsIds);
        void SetActive(bool value);
    }
}
