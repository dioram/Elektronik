using Grpc.Core;
using System;
using Elektronik.Common.Data.Pb;

namespace Protobuf.Tests
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
    }
}
