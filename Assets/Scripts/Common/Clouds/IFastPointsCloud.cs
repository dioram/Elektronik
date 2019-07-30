using UnityEngine;

namespace Elektronik.Common.Clouds
{
    public interface IFastPointsCloud
    {
        bool Exists(int idx);
        void Get(int idx, out Vector3 position, out Color color);
        void Set(int idx, Matrix4x4 offset, Color color);
        void Set(int idx, Color color);
        void Set(int[] idxs, Matrix4x4[] offsets, Color[] colors);
        void SetActive(bool value);
        void Clear();
        void Repaint();
        void GetAll(out int[] indices, out Vector3[] positions, out Color[] colors);
    }
}
