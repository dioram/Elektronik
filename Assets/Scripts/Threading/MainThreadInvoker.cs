using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            var w = Stopwatch.StartNew();
            var actionsAmount = Actions.Count;
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
            w.Stop();
            if (w.ElapsedMilliseconds > 30)
            {
                Debug.LogError($"WARNING!!! Main thread invoker took {w.ElapsedMilliseconds} milliseconds" +
                               $" in main thread for {actionsAmount} action(s).");
            }
        }
    }
}