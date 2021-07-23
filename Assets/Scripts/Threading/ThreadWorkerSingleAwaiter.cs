using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Elektronik.Threading
{
    /// <summary>
    /// Runs tasks in another thread.
    /// Ensures that only one task is running and only one task is waiting.
    /// If new task enqueued when another is waiting then old task disposes and new one waiting;
    /// </summary>
    public class ThreadWorkerSingleAwaiter
    {
        private Action _waitingTask;
        private bool _isRunning = false;
        private readonly object _locker = new object();

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

        private void RunTasks(Action action)
        {
            lock (_locker)
            {
                _isRunning = true;
            }
            action?.Invoke();

            while (!(_waitingTask is null))
            {
                Action tmp;
                lock (_locker)
                {
                    tmp = _waitingTask;
                    _waitingTask = null;
                }

                try
                {
                    tmp?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            lock (_locker)
            {
                _isRunning = false;
            }
        }
    }
}