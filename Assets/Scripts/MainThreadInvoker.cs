using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Elektronik
{
    public class MainThreadInvoker : MonoBehaviour
    {
        public static MainThreadInvoker Instance { get; private set; }

        private Thread _mainThread;

        public AutoResetEvent Sync { get; private set; }

        private Queue<Action> _actions;

        private void Awake()
        {
            _mainThread = Thread.CurrentThread;
            _actions = new Queue<Action>();
            Sync = new AutoResetEvent(false);
        }

        private void Start()
        {
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
                lock (_actions)
                {
                    _actions.Enqueue(action);
                }
            }
            else
            {
                action();
            }
        }

        private void Update()
        {
            lock(_actions)
            {
                while (_actions.Count != 0)
                {
                    _actions.Dequeue()();
                }
            }
            Sync.Set();
        }

    }
}
