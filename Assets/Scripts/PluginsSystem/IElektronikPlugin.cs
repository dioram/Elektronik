using Elektronik.Settings;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPlugin
    {
        /// <summary> Name to display in plugins settings. </summary>
        string DisplayName { get; }
        
        /// <summary> Plugins description. Will be displayed in plugins settings </summary>
        /// <remarks> Supports unity rich text. See: https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html </remarks>
        string Description { get; }
        
        /// <summary> Plugins settings. </summary>
        SettingsBag Settings { get; set; }
        
        /// <summary> Container for settings history. </summary>
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