using Elektronik.Settings;
using JetBrains.Annotations;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPlugin
    {
        /// <summary> Name to display in plugins settings. </summary>
        [NotNull]
        string DisplayName { get; }

        /// <summary> Plugins description. Will be displayed in plugins settings </summary>
        /// <remarks> Supports unity rich text. See: http://digitalnativestudios.com/textmeshpro/docs/rich-text/ </remarks>
        [NotNull]
        string Description { get; }

        /// <summary> Plugins settings. </summary>
        [NotNull]
        SettingsBag Settings { get; set; }

        /// <summary> Container for settings history. </summary>
        [NotNull]
        ISettingsHistory SettingsHistory { get; }

        /// <summary> Starts plugin. </summary>
        void Start();

        /// <summary> Stops plugin. </summary>
        void Stop();

        /// <summary> Calls every time when Unity.Update() event happens. </summary>
        /// <param name="delta"> Time from previous update call in seconds. </param>
        void Update(float delta);
    }
}