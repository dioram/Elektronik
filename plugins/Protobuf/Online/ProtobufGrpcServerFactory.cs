using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Grpc.Core.Logging;

namespace Elektronik.Protobuf.Online
{
    public class ProtobufGrpcServerFactory : ElektronikPluginsFactoryBase<OnlineSettingsBag>,
                                             IDataSourcePluginsOnlineFactory
    {
        public override IElektronikPlugin Start(ICSConverter converter)
        {
            return new ProtobufGrpcServer(TypedSettings, converter, Logger);
        }

        public ILogger? Logger;

        public override string DisplayName => "Protobuf";

        public override string Description =>
                "This plugin plays data coming through " +
                "<#7f7fe5><u><link=\"https://grpc.io/\">gRPC</link></u></color> with " +
                "<#7f7fe5><u><link=\"https://developers.google.com/protocol-buffers/\">" +
                "Protocol buffers</link></u></color>. " +
                "You can find documentation for data package format " +
                "<#7f7fe5><u><link=\"https://github.com/dioram/Elektronik-Tools-2.0/blob/master/docs/Protobuf-EN.md\">" +
                "here</link></u></color>. Also you can see *.proto files in <ElektronikDir>/Plugins/Protobuf/data.";

        public override string Version => "1.3";
    }
}