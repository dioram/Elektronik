using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Elektronik.Threading
{
    public interface IMainThreadInvoker
    {
        public void Enqueue(Action action);
    }
    
    public class MainThreadInvoker : MonoBehaviour, IMainThreadInvoker
    {
        public static IMainThreadInvoker Instance { get; private set; }

        private static Thread _mainThread;

        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

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

        public void Enqueue(Action action)
        {
            if (Thread.CurrentThread != _mainThread)
            {
                _actions.Enqueue(action);
                return;
            }

            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Update()
        {
            var w = Stopwatch.StartNew();
            var actionsAmount = _actions.Count;
            while (_actions.Count != 0)
            {
                if (!_actions.TryDequeue(out Action a)) continue;
                
                try
                {
                    a?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
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