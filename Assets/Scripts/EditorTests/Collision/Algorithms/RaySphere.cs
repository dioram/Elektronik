using Elektronik.DataConsumers.Collision;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests.Collision.Algorithms
{
    public class RaySphere
    {
        [Test]
        public void RayThroughCenter()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var center = Vector3.forward;
            var radius = 0.2f;
            
            Assert.IsTrue(CollisionAlgorithms.RaySphere(ray, center, radius));
        }

        [Test]
        public void RayInRadius()
        {
            
            var ray = new Ray(new Vector3(0.1f, 0.1f, 0), Vector3.forward);
            var center = Vector3.forward;
            var radius = 0.2f;
            
            Assert.IsTrue(CollisionAlgorithms.RaySphere(ray, center, radius));
        }

        [Test]
        public void RayNotInRadius()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var center = new Vector3(0.3f, 0, 1f);
            var radius = 0.2f;
            
            Assert.IsFalse(CollisionAlgorithms.RaySphere(ray, center, radius));
        }

        [Test]
        public void RayFromInside()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var center = Vector3.zero;
            var radius = 0.2f;
            
            Assert.IsTrue(CollisionAlgorithms.RaySphere(ray, center, radius));
        }

        [Test]
        public void SphereBehindOrigin()
        {
            var ray = new Ray(Vector3.zero, Vector3.forward);
            var center = Vector3.back;
            var radius = 0.2f;
            
            Assert.IsFalse(CollisionAlgorithms.RaySphere(ray, center, radius));
        }
    }
}