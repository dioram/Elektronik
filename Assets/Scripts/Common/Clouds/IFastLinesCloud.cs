using Elektronik.Common.Clouds.Meshes;
using System.Collections.Generic;
using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public interface IFastLinesCloud
    {
        void Clear();
        bool Exists(int lineIdx);
        CloudLine Get(int idx);
        void Add(CloudPoint point);
        void Add(IEnumerable<CloudPoint> points);
        void UpdatePoint(CloudPoint point);
        void UpdatePoints(IEnumerable<CloudPoint> points);
        void Remove(int idx);
        void Remove(IEnumerable<int> pointsIds);
    }
}