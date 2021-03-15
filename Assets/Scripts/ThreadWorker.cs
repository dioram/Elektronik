using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Elektronik
{
    public class ThreadWorker : IDisposable
    {
        private readonly Thread _thread;
        private readonly Queue<Action> _actions = new Queue<Action>();

        public int QueuedActions { get; private set; }

        public ThreadWorker()
        {
            _thread = new Thread(Start);
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
                QueuedActions++;
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
                        try
                        {
                            _actions.Dequeue()();
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                        finally
                        {
                            QueuedActions--;  
                        }
                    }
                }

                Thread.Sleep(20);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}