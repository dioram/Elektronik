using System;
using System.IO;
using Elektronik.Protobuf.Data;
using Google.Protobuf;
using Grpc.Core;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public abstract class TestsBase
    {
        protected readonly MapsManagerPb.MapsManagerPbClient MapClient;
        protected readonly ImageManagerPb.ImageManagerPbClient ImageClient;

        protected TestsBase()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = new Channel("127.0.0.1:5050", ChannelCredentials.Insecure);
            MapClient = new MapsManagerPb.MapsManagerPbClient(channel);
            ImageClient = new ImageManagerPb.ImageManagerPbClient(channel);
        }

        protected void SendAndCheck(PacketPb packet, string? filename = null, bool isFirst = false)
        {
            if (filename != null)
            {
                using var file = File.Open(filename, isFirst ? FileMode.Create : FileMode.Append);
                packet.WriteDelimitedTo(file);
            }
            var response = MapClient.Handle(packet);
            Assert.True(response.ErrType == ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}
