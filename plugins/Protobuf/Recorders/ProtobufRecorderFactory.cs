using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRecorderFactory : ElektronikPluginsFactoryBase<SettingsBag>, IDataRecorderFactory
    {
        public override IElektronikPlugin Start(ICSConverter converter) => new ProtobufRecorder(converter);
        public override string DisplayName => "Recorder to Protobuf";
        public override string Description => "Records data to Protobuf file";
        public override string Version => "1.3";
    }
}