using Elektronik.DataConsumers.Collision;
using NUnit.Framework;
using UnityEngine;

namespace Elektronik.EditorTests
{
    public class CollisionCloudTests
    {
        [Test]
        public void PointFound()
        {
            var block = new CollisionBlock(Vector3Int.zero);
            block.AddItem(0, Vector3.zero);
            var ray = new Ray(Vector3.back, Vector3.forward);
            
            Assert.AreEqual(0, block.FindItem(ray, 0.2f).Value);
        }

        [Test]
        public void PointFoundInSecondBlock()
        {
            var block = new CollisionBlock(Vector3Int.zero);
            block.AddItem(0, Vector3.up * 5 + Vector3.forward * 0.3f);
            block.AddItem(1, Vector3.up * 15);
            var ray = new Ray(Vector3.zero, Vector3.up);

            Assert.AreEqual(1, block.FindItem(ray, 0.2f).Value);
        }

        [Test]
        public void PointNotFound()
        {
            var block = new CollisionBlock(Vector3Int.zero);
            block.AddItem(0, Vector3.zero);
            var ray = new Ray(Vector3.back, Vector3.one);
            
            Assert.IsFalse(block.FindItem(ray, 0.2f).HasValue);
        }
        
        [Test]
        public void Remove()
        {
            var block = new CollisionBlock(Vector3Int.zero);
            block.AddItem(0, Vector3.left * 5);
            block.AddItem(1, Vector3.up * 15);
            
            var ray = new Ray(Vector3.zero, Vector3.up);
            Assert.AreEqual(1, block.FindItem(ray, 0.2f).Value);

            block.RemoveItem(1, Vector3.up * 15);
            Assert.IsFalse(block.FindItem(ray, 0.2f).HasValue);
        }
        
        [Test]
        public void Update()
        {
            var block = new CollisionBlock(Vector3Int.zero);
            block.AddItem(0, Vector3.left);
            
            var ray = new Ray(Vector3.zero, Vector3.left);
            Assert.AreEqual(0, block.FindItem(ray, 0.2f).Value);

            block.UpdateItem(0, Vector3.left, Vector3.one);
            Assert.IsFalse(block.FindItem(ray, 0.2f).HasValue);
            
            ray = new Ray(Vector3.zero, Vector3.one);
            Assert.AreEqual(0, block.FindItem(ray, 0.2f).Value);
        }
        
        [Test]
        public void Clear()
        {
            var block = new CollisionBlock(Vector3Int.zero);
            block.AddItem(0, Vector3.left * 5);
            block.AddItem(1, Vector3.up * 15);
            
            var ray0 = new Ray(Vector3.zero, Vector3.left);
            Assert.AreEqual(0, block.FindItem(ray0, 0.2f).Value);
            
            var ray1 = new Ray(Vector3.zero, Vector3.up);
            Assert.AreEqual(1, block.FindItem(ray1, 0.2f).Value);

            block.Clear();
            Assert.IsFalse(block.FindItem(ray0, 0.2f).HasValue);
            Assert.IsFalse(block.FindItem(ray1, 0.2f).HasValue);
        }
    }
}