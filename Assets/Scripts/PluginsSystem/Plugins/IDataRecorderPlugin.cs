using System;
using Elektronik.DataConsumers;

namespace Elektronik.PluginsSystem
{
    // TODO: add xml docs
    public interface IDataRecorderPlugin : IElektronikPlugin, IDataConsumer
    {
        public event Action OnDisposed;
    }
}