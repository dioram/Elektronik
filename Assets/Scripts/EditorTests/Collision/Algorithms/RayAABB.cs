using Elektronik.DataConsumers.Collision;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests.Collision.Algorithms
{
    public class RayAABB
    {
        [Test]
        public void Collided()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var leftBottom = new Vector3(-1, -1, 1);
            var rightTop = new Vector3(1, 1, 2);
            
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) > 0);
        }

        [Test]
        public void NotCollided()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var leftBottom = new Vector3(0.5f, 0.5f, 1);
            var rightTop = new Vector3(1, 1, 2);
            
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) < 0);
        }

        [Test]
        public void RayFromInside()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var leftBottom = new Vector3(-1, -1, -1);
            var rightTop = new Vector3(1, 1, 2);
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) == 0);
        }

        [Test]
        public void AABB_BehindOrigin()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var leftBottom = new Vector3(-1, -1, -1);
            var rightTop = new Vector3(-0.1f, -0.1f, -0.1f);
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) < 0);
        }
    }
}