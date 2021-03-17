using System.Linq;
using System.Text;
using Elektronik.Ros.Rosbag.Parsers;
using NUnit.Framework;

namespace Elektronik.Ros.Tests.Rosbag
{
    public class BitConversionTests
    {
        [Test]
        public void ByteSplitTest()
        {
            var data = "name=field=data"
                    .Select(c => (byte) c)
                    .ToArray()
                    .Split((byte) '=')
                    .Select(a => Encoding.UTF8.GetString(a))
                    .ToArray();
            
            Assert.AreEqual(new [] {"name", "field", "data"}, data);
        }
    }
}