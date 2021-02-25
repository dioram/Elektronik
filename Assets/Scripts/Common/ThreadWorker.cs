using System;
using System.Collections.Generic;
using System.Threading;

namespace Elektronik.Common
{
    public class ThreadWorker : IDisposable
    {
        private readonly Thread _thread;
        private readonly Queue<Action> _actions;

        public ThreadWorker()
        {
            _thread = new Thread(Start);
            _actions = new Queue<Action>();
            _thread.Start();
        }

        public void Dispose()
        {
            _thread.Abort();
        }

        public void Enqueue(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }

        private void Start()
        {
            while (true)
            {
                lock (_actions)
                {
                    while (_actions.Count > 0)
                    {
                        _actions.Dequeue()();
                    }
                }

                Thread.Sleep(20);
            }
        }
    }
}