using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.Settings.Bags;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRecorderFactory : ElektronikPluginsFactoryBase<SettingsBag>, IDataRecorderFactory
    {
        protected override IElektronikPlugin StartPlugin(SettingsBag settings, ICSConverter converter)
        {
            return new ProtobufRecorder(DisplayName, Logo, converter);
        }

        public override string DisplayName => "Recorder to Protobuf";
        public override string Description => "Records data to Protobuf file";
        public string Extension => ".dat";
        public bool StartsFromSceneLoading => false;
    }
}