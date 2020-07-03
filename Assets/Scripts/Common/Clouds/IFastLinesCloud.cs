using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public interface IFastLinesCloud
    {
        void Clear();
        bool Exists(int lineIdx);
        void Get(int idx, out Vector3 position1, out Vector3 position2, out Color color);
        void Set(int idx, Color color);
        void Set(int idx, Color color1, Color color2);
        void Set(int idx, Vector3 position1, Vector3 position2);
        void Set(int idx, Vector3 position1, Vector3 position2, Color color);
        void Set(int idx, Vector3 position1, Vector3 position2, Color color1, Color color2);
        void Set(int[] idxs, Vector3[] positions1, Vector3[] positions2, Color[] colors);
        void SetActive(bool value);
    }
}