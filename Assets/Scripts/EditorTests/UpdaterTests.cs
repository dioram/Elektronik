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

        [Test, Sequential]
        public void IsVersionNewer(
            [Values("1", "1", "2.0", "2.2", "2.1.1", "2.2.8", "2.1.0", "2.1.0-rc3", "2.1.0-rc3")]
            string first,
            [Values("0", "0.1", "1.5", "2.1", "2.1.0", "2.2.3", "2.1.0-rc8", "2.1.0-rc1", "2.1.0-rc3-WIP")]
            string second)
        {
            Assert.IsTrue(new Updater.Version(first) > new Updater.Version(second));
            Assert.IsFalse(new Updater.Version(first) < new Updater.Version(second));
            Assert.IsFalse(new Updater.Version(second) > new Updater.Version(first));
            Assert.IsTrue(new Updater.Version(second) < new Updater.Version(first));
            Assert.IsFalse(new Updater.Version(first) == new Updater.Version(second));
            Assert.IsFalse(new Updater.Version(first).Equals(new Updater.Version(second)));
            Assert.IsTrue(new Updater.Version(first) != new Updater.Version(second));
        }

        [Test, Sequential]
        public void IsVersionSame([Values("0", "1", "1.1")] string first,
                                  [Values("0.0", "1.0", "1.1.0")] string second)
        {
            Assert.IsTrue(new Updater.Version(first) == new Updater.Version(second));
            Assert.IsTrue(new Updater.Version(first).Equals(new Updater.Version(second)));
            
            Assert.IsFalse(new Updater.Version(first) != new Updater.Version(second));
            Assert.IsFalse(new Updater.Version(first) < new Updater.Version(second));
            Assert.IsFalse(new Updater.Version(first) > new Updater.Version(second));
            Assert.IsFalse(new Updater.Version(second) < new Updater.Version(first));
            Assert.IsFalse(new Updater.Version(second) > new Updater.Version(first));
        }
    }
}