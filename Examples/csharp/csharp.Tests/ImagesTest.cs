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
        private string filename = $"{nameof(ImagesTest)}.dat";

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

                var response = m_imageClient.Handle(packet);
                Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
            
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void OfflineImage()
        {
            var f = File.Open(filename, FileMode.Create);

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