using UnityEngine;

namespace Elektronik.Extensions
{
    public static class VectorExtensions
    {
        public static bool IsCorrect(this Vector3 vector)
        {
            return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z)
                    && !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
        }
    }
}