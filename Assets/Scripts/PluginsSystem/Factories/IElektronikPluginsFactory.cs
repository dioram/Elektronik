using Elektronik.Data.Converters;
using Elektronik.Settings;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    /// <summary>
    /// This interface is base for all plugins factories.
    /// It's main goal to produce plugin.
    /// All classes with this interface will be loaded from Plugins/[PluginName]/libraries directory and instantiated.
    /// </summary>
    /// <remarks> It's better to inherit <c>ElektronikPluginsFactoryBase</c> than implement this interface. </remarks>
    public interface IElektronikPluginsFactory
    {
        /// <summary> Logo of the plugin. Will be displayed in connections window and toolbar. </summary>
        /// <remarks> Should be set by <c>void LoadLogo(string path)</c> </remarks>
        Texture2D Logo { get; }

        /// <summary> Loads plugin's logo from file. </summary>
        /// <param name="path"> Path to logo file. </param>
        void LoadLogo(string path);
        
        /// <summary> Name to display in connections window. </summary>
        string DisplayName { get; }

        /// <summary> Plugins description. Will be displayed in connections window. </summary>
        /// <remarks> Supports unity rich text. See: http://digitalnativestudios.com/textmeshpro/docs/rich-text/ </remarks>
        string Description { get; }

        /// <summary> Settings of plugin that should be set before its start. </summary>
        SettingsBag Settings { get; set; }
        
        /// <summary> Container for settings. </summary>
        ISettingsHistory SettingsHistory { get; }

        /// <summary> This function creates and starts plugin. </summary>
        /// <param name="converter"> Converter to unity coordinate system. </param>
        /// <returns> Instantiated plugin. </returns>
        IElektronikPlugin Start(ICSConverter converter);
    }
}