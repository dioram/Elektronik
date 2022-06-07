using Elektronik.PluginsSystem;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayerFactory : ElektronikPluginsFactoryBase<OfflineSettingsBag>,
                                             ISnapshotReaderPluginsFactory
    {
        public override string DisplayName => "Protobuf";
        public override string Description => "This plugin reads " +
            "<#7f7fe5><u><link=\"https://developers.google.com/protocol-buffers/\">protobuf</link></u></color>" +
            " packages from file. You can find documentation for data package format " +
            "<#7f7fe5><u><link=\"https://github.com/dioram/Elektronik-Tools-2.0/blob/master/docs/Protobuf-EN.md\">" +
            "here</link></u></color>. Also you can see *.proto files in <ElektronikDir>/Plugins/Protobuf/data.";

        public void SetFileName(string? path)
        {
            _filepath = path;
        }

        public string[] SupportedExtensions { get; } = {".dat"};

        protected override IElektronikPlugin StartPlugin(OfflineSettingsBag settings)
        {
            if (!string.IsNullOrEmpty(_filepath))
            {
                settings.PathToFile = _filepath!;
            }
            return new ProtobufFilePlayer(DisplayName, Logo, settings);
        }

        private string? _filepath;
    }
}