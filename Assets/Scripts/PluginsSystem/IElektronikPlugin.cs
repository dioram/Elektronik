using System;
using Elektronik.Settings.Bags;
using JetBrains.Annotations;
using UnityEngine;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPlugin: IDisposable
    {
        /// <summary> Name to display in toolbar. </summary>
        string DisplayName { get; }
        
        SettingsBag Settings { get; }
        
        [CanBeNull] Texture2D Logo { get; }
        
        /// <summary> Calls every time when Unity.Update() event happens. </summary>
        /// <param name="delta"> Time from previous update call in seconds. </param>
        void Update(float delta);
    }
}