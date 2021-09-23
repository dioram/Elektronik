using Elektronik.DataConsumers.Collision;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests
{
    public class CollisionAlgorithmsTests
    {
        [Test]
        public void RaySphere()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var center = Vector3.forward;
            var radius = 0.2f;
            
            Assert.IsTrue(CollisionAlgorithms.RaySphere(ray, center, radius));
            
            center += Vector3.up * 0.3f;
            Assert.IsFalse(CollisionAlgorithms.RaySphere(ray, center, radius));
            
            center = Vector3.back;
            Assert.IsFalse(CollisionAlgorithms.RaySphere(ray, center, radius));
            
            center = Vector3.forward;
            ray.direction = Vector3.one;
            Assert.IsFalse(CollisionAlgorithms.RaySphere(ray, center, radius));
            
            center = Vector3.one * 300;
            Assert.IsTrue(CollisionAlgorithms.RaySphere(ray, center, radius));
        }

        [Test]
        public void RayAABB()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var leftBottom = new Vector3(-1, -1, 1);
            var rightTop = new Vector3(1, 1, 2);
            
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) > 0);
            
            leftBottom = new Vector3(0.5f, 0.5f, 1);
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) < 0);
            
            leftBottom = new Vector3(-1, -1, -1);
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) == 0);
            
            leftBottom = new Vector3(-1, -1, -1);
            rightTop = new Vector3(-0.1f, -0.1f, -0.1f);
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) < 0);

            leftBottom = new Vector3(-1, -1, 1.1f);
            rightTop = new Vector3(1, 1, 2.1f);
            ray = new Ray(Vector3.zero, Vector3.one);
            Assert.IsTrue(CollisionAlgorithms.RayAABB(ray, leftBottom, rightTop) < 0);
        }
    }
}