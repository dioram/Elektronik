using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Elektronik.Threading
{
    public class MainThreadInvoker : MonoBehaviour
    {
        public static MainThreadInvoker Instance { get; private set; }

        private static Thread _mainThread;

        private static readonly ConcurrentQueue<Action> Actions = new ConcurrentQueue<Action>();

        private void Awake()
        {
            _mainThread = Thread.CurrentThread;
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

        public static void Enqueue(Action action)
        {
            if (Thread.CurrentThread != _mainThread)
            {
                Actions.Enqueue(action);
            }
            else
            {
                action();
            }
        }

        private void Update()
        {
            while (Actions.Count != 0)
            {
                if (Actions.TryDequeue(out Action a))
                {
                    try
                    {
                        a?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}