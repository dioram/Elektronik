using UnityEngine;

namespace Elektronik.Common.Maps
{
    public abstract class RepaintableObject : MonoBehaviour
    {
        public abstract void Repaint();
        public abstract void Clear();
    }
}
