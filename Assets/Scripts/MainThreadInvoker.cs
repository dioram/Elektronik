using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Elektronik
{
    public class MainThreadInvoker : MonoBehaviour
    {
        public static MainThreadInvoker Instance { get; private set; }

        private Thread _mainThread;

        private ConcurrentQueue<Action> _actions;

        private void Awake()
        {
            _mainThread = Thread.CurrentThread;
            _actions = new ConcurrentQueue<Action>();
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("there can't be more than one MainThreadInvoker in each scene");
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Enqueue(Action action)
        {
            if (Thread.CurrentThread != _mainThread)
            {
                _actions.Enqueue(action);
            }
            else
            {
                action();
            }
        }

        private void Update()
        {
            while (_actions.Count != 0)
            {
                if (_actions.TryDequeue(out Action a))
                {
                    a();
                }
            }
        }
    }
}