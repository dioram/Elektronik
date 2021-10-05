using System;
using Elektronik.Threading;

namespace Elektronik.Plugins.Common
{
    public class FakeMainThreadInvoker : IMainThreadInvoker
    {
        public FakeMainThreadInvoker()
        {
            MainThreadInvoker.Instance = this;
        }
        
        public void Enqueue(Action action)
        {}
    }
}