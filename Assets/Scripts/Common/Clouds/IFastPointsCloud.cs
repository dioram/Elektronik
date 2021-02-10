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
        void Add(CloudPoint point);
        void Add(IEnumerable<CloudPoint> points);
        void UpdatePoint(CloudPoint point);
        void UpdatePoints(IEnumerable<CloudPoint> points);
        void Remove(int idx);
        void Remove(IEnumerable<int> pointsIds);
        void SetActive(bool value);
    }
}
