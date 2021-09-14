using System;

namespace Elektronik.PluginsSystem
{
    // TODO: add xml docs
    public interface IDataRecorderPlugin : IElektronikPlugin, ISourceRenderer
    {
        public event Action OnDisposed;
    }
}