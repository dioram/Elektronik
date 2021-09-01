using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;

namespace Elektronik.Protobuf.Offline
{
    public class ProtobufFilePlayerFactory : ElektronikPluginsFactoryBase<OfflineSettingsBag>,
                                             IDataSourcePluginsOfflineFactory
    {
        public override string DisplayName { get; } = "Protobuf";
        public override string Description { get; } = "This plugin reads " +
            "<#7f7fe5><u><link=\"https://developers.google.com/protocol-buffers/\">protobuf</link></u></color>" +
            " packages from file. You can find documentation for data package format " +
            "<#7f7fe5><u><link=\"https://github.com/dioram/Elektronik-Tools-2.0/blob/master/docs/Protobuf-EN.md\">" +
            "here</link></u></color>. Also you can see *.proto files in <ElektronikDir>/Plugins/Protobuf/data.";

        public override string Version { get; } = "1.3";
        public void SetFileName(string path)
        {
            TypedSettings.FilePath = path;
        }

        public string[] SupportedExtensions { get; } = {".dat"};

        public override IElektronikPlugin Start(ICSConverter converter)
        {
            return new ProtobufFilePlayer(TypedSettings, converter);
        }
    }
}