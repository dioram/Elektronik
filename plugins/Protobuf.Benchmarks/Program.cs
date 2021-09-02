using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Protobuf.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // BenchmarkRunner.Run<ClearBenchmark>();
            // BenchmarkRunner.Run<OnlineOfflineBenchmark>(new DebugInProcessConfig());
            BenchmarkRunner.Run<OnlineOfflineBenchmark>();
        }
    }
}