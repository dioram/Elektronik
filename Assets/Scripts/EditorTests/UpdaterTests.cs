using NUnit.Framework;

namespace Elektronik.EditorTests
{
    [TestFixture]
    public class UpdaterTests
    {
        [Test]
        public void IsVersionNewer()
        {
            Assert.IsTrue(new Updater.Version("2.0") > new Updater.Version("1.5"));
            Assert.IsFalse(new Updater.Version("2") > new Updater.Version("2.0"));
            Assert.IsTrue(new Updater.Version("2.2") > new Updater.Version("2.1"));
            Assert.IsFalse(new Updater.Version("2.1.0") > new Updater.Version("2.1"));
            Assert.IsTrue(new Updater.Version("2.1.2") > new Updater.Version("2.1.1"));
            Assert.IsTrue(new Updater.Version("2.1.0") > new Updater.Version("2.1.0-rc8"));
            Assert.IsTrue(new Updater.Version("2.1.0-rc3") > new Updater.Version("2.1.0-rc1"));
            Assert.IsTrue(new Updater.Version("2.1.0-rc3") > new Updater.Version("2.1.0-rc3-WIP"));

            Assert.IsFalse(new Updater.Version("1.5") > new Updater.Version("2.0"));
            Assert.IsFalse(new Updater.Version("2.1") > new Updater.Version("2.2"));
            Assert.IsFalse(new Updater.Version("2.1.1") > new Updater.Version("2.1.2"));
            Assert.IsFalse(new Updater.Version("2.1.0-rc8") > new Updater.Version("2.1.0"));
            Assert.IsFalse(new Updater.Version("2.1.0-rc1") > new Updater.Version("2.1.0-rc3"));
            Assert.IsFalse(new Updater.Version("2.1.0-rc3-WIP") > new Updater.Version("2.1.0-rc3"));
        }
    }
}