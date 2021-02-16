using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Elektronik.Common
{
    public class MainThreadInvoker : MonoBehaviour
    {
        public static MainThreadInvoker Instance { get; private set; }

        private Thread m_mainThread;

        public AutoResetEvent Sync { get; private set; }

        private Queue<Action> m_actions;

        private void Awake()
        {
            m_mainThread = Thread.CurrentThread;
            m_actions = new Queue<Action>();
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
            if (Thread.CurrentThread != m_mainThread)
            {
                lock (m_actions)
                {
                    m_actions.Enqueue(action);
                }
            }
            else
            {
                action();
            }
        }

        private void Update()
        {
            lock(m_actions)
            {
                while (m_actions.Count != 0)
                {
                    m_actions.Dequeue()();
                }
            }
            Sync.Set();
        }

    }
}
