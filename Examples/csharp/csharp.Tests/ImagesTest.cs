using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.Common.Data.Pb;
using Google.Protobuf;
using NUnit.Framework;

namespace csharp.Tests
{
    public class ImagesTest : TestsBase
    {
        void Send(byte[] array, int width, int height)
        {
            var packet = new ImagePacketPb
            {
                Height = height,
                Width = width,
                ImageData = ByteString.CopyFrom(array, 0, array.Length),
            };

            var response = m_imageClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
        
        [Test, Order(1)]
        public void SendImage()
        {
            int width = 500;
            int height = 500;

            for (int i = 1; i < 4; i++)
            {
                byte[] array = File.ReadAllBytes($"{i}.png");
                Send(array, width, height);
            
                Thread.Sleep(1000);
            }
        }
    }
}