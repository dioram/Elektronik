using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRetranslatorFactory : ElektronikPluginsFactoryBase<SettingsBag>, ICustomRecorderPluginsFactory
    {
        protected override IElektronikPlugin StartPlugin(SettingsBag settings, ICSConverter converter)
        {
            return new ProtobufRetranslator(DisplayName, Logo, converter);
        }

        public override string DisplayName => "Protobuf retranslator";
        public override string Description => "Transmit data from this instance of Elektronik to another";
    }
}