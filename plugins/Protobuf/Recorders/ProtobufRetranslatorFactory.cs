using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRetranslatorFactory: ElektronikPluginsFactoryBase<AddressesSettingsBag>, IDataRecorderFactory
    {
        public override IElektronikPlugin Start(ICSConverter converter)
        {
            return new ProtobufRetranslator(TypedSettings, converter);
        }

        public override string DisplayName => "Protobuf retranslator";
        public override string Description => "Allows Elektronik to transmit data from one instance to another";
        public override string Version => "1.3";
    }
}