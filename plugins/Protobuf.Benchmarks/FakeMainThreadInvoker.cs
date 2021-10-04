using System;
using Elektronik.Threading;

namespace Protobuf.Benchmarks
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