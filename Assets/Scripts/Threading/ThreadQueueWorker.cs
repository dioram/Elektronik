using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Threading
{
    public class ThreadQueueWorker : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private bool _disposed = false;

        public int ActiveActions { get; private set; } = 0;
        
        public void Enqueue(Action action)
        {
            if (_disposed) return;
            Task.Run(() => RunSingleAction(action));
        }

        public void Dispose()
        {
            _semaphoreSlim?.Dispose();
            _disposed = true;
        }

        private async void RunSingleAction(Action action)
        {
            ActiveActions++;
            await _semaphoreSlim.WaitAsync();
            if (_disposed) return;
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                ActiveActions--;
                _semaphoreSlim.Release();
            }
        }

    }
}