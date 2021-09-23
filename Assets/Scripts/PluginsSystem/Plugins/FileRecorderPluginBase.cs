using Elektronik.Data.Converters;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    public abstract class FileRecorderPluginBase : DataRecorderPluginBase
    {
        public FileRecorderPluginBase(string filename, ICSConverter converter)
        {
            Filename = filename;
            Converter = converter;
        }

        #region DataRecorder

        public override string DisplayName => "";
        public override SettingsBag Settings => null;
        public override Texture2D Logo => null;

        #endregion

        #region Protected

        protected readonly string Filename;
        protected readonly ICSConverter Converter;

        #endregion
    }
}