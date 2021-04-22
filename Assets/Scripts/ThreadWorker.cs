using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Elektronik
{
    public class ThreadWorker : IDisposable
    {
        private readonly Thread _thread;
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        public int AmountOfActions = 0;

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
            AmountOfActions++;
            _actions.Enqueue(action);
        }

        private void Start()
        {
            while (true)
            {
                if (_actions.TryDequeue(out Action a))
                {
                    try
                    {
                        a();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    finally
                    {
                        AmountOfActions--;
                    }
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}