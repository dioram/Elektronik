using System;
using Elektronik.Extensions;
using NUnit.Framework;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace Elektronik.EditorTests
{
    public class LinqExtensionsTests
    {
        struct TestStruct
        {
            public int Id;
            public float F;
        }
        
        [Test]
        public void MaxByFound()
        {
            var list = new []
            {
                new TestStruct {Id = 0, F = 15f,},
                new TestStruct {Id = 10, F = 2f,},
            };

            var item = list.MaxBy(i => i.F);
            
            Assert.AreEqual(list[0], item);
        }

        [Test]
        public void MaxByEmpty()
        {
            var list = Array.Empty<TestStruct>();
            Assert.Catch<InvalidOperationException>(() => list.MaxBy(i => i.F));
        }

        [Test]
        public void MaxByNull()
        {
            Assert.Catch<ArgumentNullException>(() => LinqExtensions.MaxBy<TestStruct, float>(null, null));
            var list = Array.Empty<TestStruct>();
            Assert.Catch<ArgumentNullException>(() => list.MaxBy<TestStruct, float>(null));
        }
        
        [Test]
        public void MinByFound()
        {
            var list = new []
            {
                new TestStruct {Id = 0, F = 15f,},
                new TestStruct {Id = 10, F = 2f,},
            };

            var item = list.MinBy(i => i.F);
            
            Assert.AreEqual(list[1], item);
        }

        [Test]
        public void MinByEmpty()
        {
            var list = Array.Empty<TestStruct>();
            Assert.Catch<InvalidOperationException>(() => list.MinBy(i => i.F));
        }

        [Test]
        public void MinByNull()
        {
            Assert.Catch<ArgumentNullException>(() => LinqExtensions.MinBy<TestStruct, float>(null, null));
            var list = Array.Empty<TestStruct>();
            Assert.Catch<ArgumentNullException>(() => list.MinBy<TestStruct, float>(null));
        }
    }
}