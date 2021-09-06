using Elektronik.Data.Converters;
using Elektronik.Settings;
using Elektronik.Settings.Bags;
using UnityEngine;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPluginsFactory
    {
        /// <summary> Logo of the plugin. Will be displayed in connections window and toolbar. </summary>
        Texture2D Logo { get; }
        
        /// <summary> Name to display in connections window. </summary>
        string DisplayName { get; }

        /// <summary> Plugins description. Will be displayed in connections window. </summary>
        /// <remarks> Supports unity rich text. See: http://digitalnativestudios.com/textmeshpro/docs/rich-text/ </remarks>
        string Description { get; }
        
        ISettingsHistory SettingsHistory { get; }

        /// <summary> Plugins settings. </summary>
        SettingsBag Settings { get; set; }

        IElektronikPlugin Start(ICSConverter converter);

        void LoadLogo(string path);
    }

    public interface IDataSourcePluginsFactory : IElektronikPluginsFactory
    {
        
    }

    public interface IFileSourcePluginsFactory : IDataSourcePluginsFactory
    {
        public string[] SupportedExtensions { get; }
    }

    public interface ISnapshotReaderPluginsFactory : IFileSourcePluginsFactory
    {
        public void SetFileName(string path);
    }

    public interface IDataRecorderFactory: IElektronikPluginsFactory
    {
        string Extension { get; }
        bool StartsFromSceneLoading { get; }
    }
}