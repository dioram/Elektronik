using System.IO;
using System.Linq;
using System.Threading;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using NUnit.Framework;

namespace Protobuf.Tests
{
    public class ImagesTests : TestsBase
    {
        private readonly string _filename = $"{nameof(ImagesTests)}.dat";

        [Test]
        public void OnlineImage()
        {
            for (int i = 1; i < 4; i++)
            {
                byte[] array = File.ReadAllBytes($"{i}.png");
                
                var packet = new ImagePacketPb
                {
                    ImageData = ByteString.CopyFrom(array, 0, array.Length),
                };

                var response = ImageClient.Handle(packet);
                Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
            
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void OfflineImage()
        {
            using var f = File.Open(_filename, FileMode.Create);

            var packets = Enumerable.Range(1, 4).Select(i => new PacketPb
            {
                Special = true,
                Timestamp = i,
                Action = PacketPb.Types.ActionType.Clear,
                Points = new PacketPb.Types.Points(),
            });

            foreach (var packet in packets)
            {
                packet.WriteDelimitedTo(f);
            }
        }
    }
}