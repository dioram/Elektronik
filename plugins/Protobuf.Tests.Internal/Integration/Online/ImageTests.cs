using System.IO;
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
            for (int i = 1; i < 4; i++)
            {
                byte[] array = File.ReadAllBytes($"{i}.png");
                
                var packet = new ImagePacketPb
                {
                    ImageData = ByteString.CopyFrom(array, 0, array.Length),
                };

                var response = ImageClient.Handle(packet);
                
                Thread.Sleep(200);
                response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
                MockedImageRenderer.Verify(r => r.Render(array), Times.Once);
            }

            Sut.AmountOfFrames.Should().Be(3);
        }
    }
}