using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class ImagesTests : TestsBase
    {
        [Test, Explicit]
        public void OnlineImage()
        {
            var packets = Enumerable.Range(0, 3).Select(_ => new PacketPb {Image = new ImagePb()}).ToArray();
            
            byte[] array = File.ReadAllBytes("1.png");
            packets[0].Image.Bytes = ByteString.CopyFrom(array, 0, array.Length);
            array = File.ReadAllBytes("2.png");
            packets[1].Image.Bytes = ByteString.CopyFrom(array, 0, array.Length);
            packets[2].Image.Path = Path.Combine(Directory.GetCurrentDirectory(), "3.png");

            foreach (var packet in packets)
            {
                var response = MapClient.Handle(packet);
                Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
                Thread.Sleep(1000);
            }
        }

        [Test, Explicit]
        public void OfflineImage()
        {
            var packets = Enumerable.Range(0, 3).Select(_ => new PacketPb {Image = new ImagePb()}).ToArray();
            byte[] array = File.ReadAllBytes("1.png");
            packets[0].Image.Bytes = ByteString.CopyFrom(array, 0, array.Length);
            array = File.ReadAllBytes("2.png");
            packets[1].Image.Bytes = ByteString.CopyFrom(array, 0, array.Length);
            packets[2].Image.Path = Path.Combine(Directory.GetCurrentDirectory(), "3.png");

            using var f = File.Open("OfflineImages.dat", FileMode.Create);
            foreach (var packet in packets)
            {
                packet.WriteDelimitedTo(f);
            }
        }
    }
}