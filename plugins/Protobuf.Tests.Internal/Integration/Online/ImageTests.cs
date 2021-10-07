using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Google.Protobuf;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class ImageTests: OnlineTestsBase
    {
        public ImageTests() : base(40014)
        {
        }

        [Test, Order(1)]
        public void SendImage()
        {
            var packets = Enumerable.Range(0, 3).Select(_ => new PacketPb {Image = new ImagePb()}).ToArray();
            
            byte[][] images =
            {
                File.ReadAllBytes("1.png"),
                File.ReadAllBytes("2.png"),
                File.ReadAllBytes("3.png"),
            };
            packets[0].Image.Bytes = ByteString.CopyFrom(images[0], 0, images[0].Length);
            packets[1].Image.Bytes = ByteString.CopyFrom(images[1], 0, images[1].Length);
            packets[2].Image.Path = Path.Combine(Directory.GetCurrentDirectory(), "3.png");

            for (var i = 0; i < images.Length; i++)
            {
                var response = MapClient.Handle(packets[i]);
                
                Thread.Sleep(200);
                response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
                MockedImageRenderer.Verify(r => r.Render(images[i]), Times.Once);
            }
            Sut.AmountOfFrames.Should().Be(3);
        }
    }
}