using System;
using Elektronik.DataConsumers;

namespace Elektronik.PluginsSystem
{
    /// <summary> Interface for plugins that record data somewhere. </summary>
    /// <remarks>
    /// It is better to inherit from <see cref="DataRecorderPluginBase"/> or <see cref="FileRecorderPluginBase"/>
    /// than implement this interface.
    /// </remarks>
    public interface IDataRecorderPlugin : IElektronikPlugin, IDataConsumer
    {
        /// <summary> This event will be raised when plugin was disposed. </summary>
        public event Action OnDisposed;
    }
}