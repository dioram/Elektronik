using System.Linq;
using Elektronik.Protobuf.Offline.Presenters;
using Elektronik.Protobuf.Online.Presenters;
using NUnit.Framework;

namespace Protobuf.Tests.Internal
{
    public class ImagePresentersTests
    {
        [Test]
        public void ConstructFileImagePresenter()
        {
            var presenter = new FileImagePresenter("Image", @"c:\\");
            Assert.AreEqual(0, presenter.Children.Count());
            Assert.AreEqual("Image", presenter.DisplayName);
        }
        
        [Test]
        public void ConstructRawImagePresenter()
        {
            var presenter = new RawImagePresenter("Image");
            Assert.AreEqual(0, presenter.Children.Count());
            Assert.AreEqual("Image", presenter.DisplayName);
        }
    }
}