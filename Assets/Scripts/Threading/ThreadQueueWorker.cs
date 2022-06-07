using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Threading
{
    /// <summary> Runs given tasks in separate thread but only one in time. </summary>
    public class ThreadQueueWorker : IDisposable
    {
        /// <summary> Adds new task to queue. </summary>
        public void Enqueue(Action action)
        {
            if (_disposed) return;
            Task.Run(() => RunSingleAction(action));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _semaphoreSlim?.Dispose();
            _disposed = true;
        }

        #region Private

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private bool _disposed = false;
        
        private async void RunSingleAction(Action action)
        {
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
                _semaphoreSlim.Release();
            }
        }

        #endregion
    }
}