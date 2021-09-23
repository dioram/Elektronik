using System;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NUnit.Framework;

namespace Protobuf.Tests.Elektronik
{
    public class SceneTests
    {
        [Test, Explicit]
        public void Clear()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = new Channel("127.0.0.1:5050", ChannelCredentials.Insecure);
            var client = new SceneManagerPb.SceneManagerPbClient(channel);
            var response = client.Clear(new Empty());
            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded, response.Message);
        }
    }
}