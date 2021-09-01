using Elektronik.Data.Converters;
using Elektronik.Settings;
using Elektronik.Settings.Bags;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPluginsFactory
    {
        /// <summary> Name to display in plugins settings. </summary>
        string DisplayName { get; }

        /// <summary> Plugins description. Will be displayed in plugins settings </summary>
        /// <remarks> Supports unity rich text. See: http://digitalnativestudios.com/textmeshpro/docs/rich-text/ </remarks>
        string Description { get; }

        string Version { get; }
        
        ISettingsHistory SettingsHistory { get; }

        /// <summary> Plugins settings. </summary>
        SettingsBag Settings { get; set; }

        IElektronikPlugin Start(ICSConverter converter);

        void SaveSettings();
    }

    public interface IDataSourcePluginsOfflineFactory: IElektronikPluginsFactory
    {
        public void SetFileName(string path);
        
        public string[] SupportedExtensions { get; }
    }

    public interface IDataSourcePluginsOnlineFactory: IElektronikPluginsFactory
    {
        
    }

    public interface IDataRecorderFactory: IElektronikPluginsFactory
    {
        
    }
}