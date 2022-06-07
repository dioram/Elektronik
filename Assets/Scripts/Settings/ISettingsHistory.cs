using System.Collections.ObjectModel;

namespace Elektronik.Settings
{
    /// <summary> Interface for containers of settings and their histories. </summary>
    public interface ISettingsHistory
    {
        /// <summary> Adds new settings bag to history. </summary>
        /// <param name="recent"> Settings bag that will be added. </param>
        void Add(SettingsBag recent);
        
        /// <summary> History of settings (from newest to latest) </summary>
        ReadOnlyCollection<SettingsBag> Recent { get; }
        
        /// <summary> Saves settings on disk. </summary>
        void Save();
    }
}