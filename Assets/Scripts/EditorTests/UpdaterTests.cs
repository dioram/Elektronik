using NUnit.Framework;

namespace Elektronik.EditorTests
{
    [TestFixture]
    public class UpdaterTests
    {
        [Test]
        public void IsVersionNewer()
        {
            Assert.IsTrue(Updater.IsNewer("2.0", "1.5"));
            Assert.IsFalse(Updater.IsNewer("2", "2.0"));
            Assert.IsTrue(Updater.IsNewer("2.2", "2.1"));
            Assert.IsFalse(Updater.IsNewer("2.1.0", "2.1"));
            Assert.IsTrue(Updater.IsNewer("2.1.2", "2.1.1"));
            Assert.IsTrue(Updater.IsNewer("2.1.0", "2.1.0-rc8"));
            Assert.IsTrue(Updater.IsNewer("2.1.0-rc3", "2.1.0-rc1"));
            Assert.IsTrue(Updater.IsNewer("2.1.0-rc3", "2.1.0-rc3-WIP"));
            
            Assert.IsFalse(Updater.IsNewer("1.5", "2.0"));
            Assert.IsFalse(Updater.IsNewer("2.1", "2.2"));
            Assert.IsFalse(Updater.IsNewer("2.1.1", "2.1.2"));
            Assert.IsFalse(Updater.IsNewer("2.1.0-rc8", "2.1.0"));
            Assert.IsFalse(Updater.IsNewer("2.1.0-rc1", "2.1.0-rc3"));
            Assert.IsFalse(Updater.IsNewer("2.1.0-rc3-WIP", "2.1.0-rc3"));
        }
    }
}