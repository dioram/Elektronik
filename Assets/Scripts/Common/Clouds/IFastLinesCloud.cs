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
        void Set(int idx, Color color1, Color color2);
        void Set(int idx, Vector3 position1, Vector3 position2);
        void Set(CloudLine line);
        void Set(IEnumerable<CloudLine> lines);
        void SetActive(bool value);
    }
}