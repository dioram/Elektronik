using Elektronik.Common.Data.Pb;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace csharp.Tests
{
    public abstract class TestsBase
    {
        protected MapsManagerPb.MapsManagerPbClient m_client;

        public TestsBase()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = new Channel("127.0.0.1:5050", ChannelCredentials.Insecure);
            m_client = new MapsManagerPb.MapsManagerPbClient(channel);
        }
    }
}
