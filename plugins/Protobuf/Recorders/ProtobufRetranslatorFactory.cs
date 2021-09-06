using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRetranslatorFactory: ElektronikPluginsFactoryBase<AddressesSettingsBag>, IDataRecorderFactory
    {
        protected override IElektronikPlugin StartPlugin(AddressesSettingsBag settings, ICSConverter converter)
        {
            return new ProtobufRetranslator(settings, converter);
        }

        public override string DisplayName => "Protobuf retranslator";
        public override string Description => "Allows Elektronik to transmit data from one instance to another";
        public string Extension => "";
        public bool StartsFromSceneLoading => true;
    }
}