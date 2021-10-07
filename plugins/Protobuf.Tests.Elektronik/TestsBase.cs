using System;
using System.IO;
using System.Threading;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Google.Protobuf;
using Grpc.Core;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public abstract class TestsBase
    {
        [TearDown]
        public void AddDelay()
        {
            Thread.Sleep(500);
        }
        
        protected readonly MapsManagerPb.MapsManagerPbClient MapClient;

        protected TestsBase()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = new Channel("127.0.0.1:5050", ChannelCredentials.Insecure);
            MapClient = new MapsManagerPb.MapsManagerPbClient(channel);
        }

        protected void SendAndCheck(PacketPb packet, string? filename = null, bool isFirst = false)
        {
            if (filename != null)
            {
                using var file = File.Open(filename, isFirst ? FileMode.Create : FileMode.Append);
                packet.WriteDelimitedTo(file);
            }
            var response = MapClient.Handle(packet);
            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}
