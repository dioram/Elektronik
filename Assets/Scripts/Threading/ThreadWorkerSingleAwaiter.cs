using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Threading
{
    /// <summary>
    /// Runs tasks in another thread.
    /// Ensures that only one task is running and only one task is waiting.
    /// If new task enqueued when another is waiting then old task disposes and new one starts waiting;
    /// </summary>
    public class ThreadWorkerSingleAwaiter
    {
        /// <summary> Adds new task to queue. </summary>
        public void Enqueue(Action action)
        {
            lock (_locker)
            {
                if (_isRunning)
                {
                    _waitingTask = action;
                    return;
                }
            }

            Task.Run(() => RunTasks(action));
        }

        #region Private

        private Action _waitingTask;
        private bool _isRunning = false;
        private readonly object _locker = new object();

        private void Invoke(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void RunTasks(Action action)
        {
            lock (_locker)
            {
                _isRunning = true;
            }
            Invoke(action);

            while (!(_waitingTask is null))
            {
                Action tmp;
                lock (_locker)
                {
                    tmp = _waitingTask;
                    _waitingTask = null;
                }
                Invoke(tmp);
            }

            lock (_locker)
            {
                _isRunning = false;
            }
        }
        #endregion
    }
}