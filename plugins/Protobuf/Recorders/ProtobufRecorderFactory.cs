using Elektronik.Data.Converters;
using Elektronik.PluginsSystem;
using Elektronik.Settings;
using UnityEngine;

namespace Elektronik.Protobuf.Recorders
{
    public class ProtobufRecorderFactory : IFileRecorderPluginsFactory
    {
        public IElektronikPlugin Start(ICSConverter converter)
        {
            return new ProtobufRecorder(Filename, converter);
        }

        public void LoadLogo(string path)
        {
            // Do nothing.
        }

        public Texture2D? Logo => null;
        public string DisplayName => "Recorder to Protobuf";
        public string Description => "Records data to Protobuf file";
        public SettingsBag? Settings { get; set; } = null;
        public ISettingsHistory? SettingsHistory => null;
        public string Extension => ".dat";
        public string Filename { get; set; } = "./proto.dat";
    }
}