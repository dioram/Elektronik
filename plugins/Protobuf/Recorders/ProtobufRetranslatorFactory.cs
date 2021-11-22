using Elektronik.PluginsSystem;
using Elektronik.Settings;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRetranslatorFactory : ElektronikPluginsFactoryBase<SettingsBag>, IDataRecorderPluginsFactory
    {
        protected override IElektronikPlugin StartPlugin(SettingsBag settings)
        {
            return new ProtobufRetranslator(DisplayName, Logo);
        }

        public override string DisplayName => "Protobuf retranslator";
        public override string Description => "Transmit data from this instance of Elektronik to another";
    }
}