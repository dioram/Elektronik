using UnityEngine;

namespace Elektronik.Collision
{
    public static class CollisionAlgorithms
    {
        /// <summary> Checks collision between ray and axis aligned bounding box. </summary>
        /// <param name="ray"> Ray. </param>
        /// <param name="leftBottom"> Point of AABB with minimal coordinates. </param>
        /// <param name="rightTop"> Point of AABB with maximum coordinates. </param>
        /// <returns> Distance from ray origin to intersection point. If &lt; 0 than collision was not found. </returns>
        public static float RayAABB(Ray ray, Vector3 leftBottom, Vector3 rightTop)
        {
            var t1 = (leftBottom.x - ray.origin.x) / ray.direction.x;
            var t2 = (rightTop.x - ray.origin.x) / ray.direction.x;
            var t3 = (leftBottom.y - ray.origin.y) / ray.direction.y;
            var t4 = (rightTop.y - ray.origin.y) / ray.direction.y;
            var t5 = (leftBottom.z - ray.origin.z) / ray.direction.z;
            var t6 = (rightTop.z - ray.origin.z) / ray.direction.z;

            var tMin = Mathf.Max(Mathf.Max(Mathf.Min(t1, t2), Mathf.Min(t3, t4)), Mathf.Min(t5, t6));
            var tMax = Mathf.Min(Mathf.Min(Mathf.Max(t1, t2), Mathf.Max(t3, t4)), Mathf.Max(t5, t6));

            if (tMax < 0) return -1;
            if (tMin > tMax) return -1;
            
            return tMin < 0 ? 0 : tMin;
        }

        /// <summary> Checks collision between ray and sphere </summary>
        /// <param name="ray"> Ray. </param>
        /// <param name="position"> Center of sphere. </param>
        /// <param name="radius"> Sphere radius.</param>
        /// <returns> true if ray intersects with sphere. </returns>
        public static bool RaySphere(Ray ray, Vector3 position, float radius)
        {
            // var k1 = position - ray.origin;
            // var a = Vector3.Angle(k1, ray.direction);
            // if (a > 90) return false;
            // var k2 = Mathf.Tan(a) * k1.magnitude;
            // return k2 <= radius;

            var k = ray.origin - position;
            var b = Vector3.Dot(k, ray.direction);
            var c = Vector3.Dot(k, k) - radius * radius;
            var d = b * b - c;
            
            if (d < 0) return false;
            var sqrtD = Mathf.Sqrt(d);
            
            var t1 = -b + sqrtD;
            var t2 = -b - sqrtD;
            var minT = Mathf.Min(t1, t2);
            var maxT = Mathf.Max(t1, t2);
            var t = (minT >= 0) ? minT : maxT;
            
            return t > 0;
        }
    }
}