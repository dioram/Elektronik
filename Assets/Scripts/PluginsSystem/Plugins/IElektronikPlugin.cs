using System;
using JetBrains.Annotations;
using UnityEngine;
using SettingsBag = Elektronik.Settings.SettingsBag;

namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for every plugin for elektronik. </summary>
    public interface IElektronikPlugin: IDisposable
    {
        /// <summary> Name to display in toolbar. </summary>
        string DisplayName { get; }
        
        /// <summary> Plugins settings. User can access them through toolbar. </summary>
        /// <remarks> Set it null if plugin should not have settings. </remarks>
        SettingsBag Settings { get; }
        
        /// <summary> Logo of the plugin. </summary>
        /// <remarks> By default logo will be loaded from [PluginDir]/data/[DisplayName]_Logo.png. </remarks>
        [CanBeNull] Texture2D Logo { get; }
        
        /// <summary> Calls every time when Unity.Update() event happens. </summary>
        /// <param name="delta"> Time from previous update call in seconds. </param>
        void Update(float delta);
    }
}