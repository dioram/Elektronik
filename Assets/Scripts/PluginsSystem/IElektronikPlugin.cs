using System;

namespace Elektronik.PluginsSystem
{
    public interface IElektronikPlugin: IDisposable
    {
        /// <summary> Calls every time when Unity.Update() event happens. </summary>
        /// <param name="delta"> Time from previous update call in seconds. </param>
        void Update(float delta);
    }
}