using UnityEngine;

namespace Elektronik.Extensions
{
    public static class VectorExtensions
    {
        /// <summary> Checks if all vector's dimensions are correct numbers (finite and not NAN). </summary>
        public static bool IsCorrect(this Vector3 vector)
        {
            for (var i = 0; i < 3; i++)
            {
                if (float.IsNaN(vector[i]) || float.IsInfinity(vector[i])) return false;
            }

            return true;
        }
    }
}