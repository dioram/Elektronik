using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Protobuf.Benchmarks
{
    [Config(typeof(Config))]
    [MemoryDiagnoser]
    public class ClearBenchmark
    {
        public class Config : ManualConfig
        {
            public Config()
            {
                // Using the WithOptions() factory method:
                this.WithOptions(ConfigOptions.DisableOptimizationsValidator);
            }
        }

        private readonly Dictionary<int, int> _dataDictionary = new ();
        private readonly Dictionary<(int, int), int> _dataDictionaryTuple = new ();
        private readonly List<int> _dataList = new ();
        
        
        public ClearBenchmark()
        {
            _dataList.Capacity = 10000;
            for (var i = 0; i < 10000; i++)
            {
                _dataDictionary[i] = i * 10;
                _dataDictionaryTuple[(i, 0)] = i * 10;
                _dataList.Add(i);
            }
        }

        [Benchmark]
        public void Dictionary()
        {
            var values = _dataDictionary.Keys.Where(p => p % 2 == 0).ToList();
        }

        [Benchmark]
        public void DictionaryTuple()
        {
            var values = _dataDictionaryTuple.Keys.Where(p => p.Item1 % 2 == 0).ToList();
        }
        
        [Benchmark]
        public void List()
        {
            var values = _dataList.Where(i => i % 2 == 0).ToList();
        }

        //
        //         private SlamPoint[] _points;
        //
        //         private readonly ConnectableObjectsContainer<SlamPoint> _pointsContainer =
        //                 new(new CloudContainer<SlamPoint>(), new SlamLinesContainer());
        //
        //         [Params(10000, 100_000, 1_000_000)] public int N;
        //
        //         [GlobalSetup]
        //         public void Setup()
        //         {
        //             var rand = new System.Random();
        //             _points = new SlamPoint[N];
        //             for (var i = 0; i < N; i++)
        //             {
        //                 _points[i] = new SlamPoint(
        //                     i * 2, new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
        //                     new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(),
        //                               (float)rand.NextDouble()),
        //                     $"{i * 2}");
        //             }
        //
        //             var pointsRenderer = new CloudRendererForBenchmark<SlamPoint, CloudBlockForBenchmark>();
        //             var linesRenderer = new CloudRendererForBenchmark<SlamLine, CloudBlockForBenchmark>();
        //             _pointsContainer.SetRenderer(pointsRenderer);
        //             _pointsContainer.SetRenderer(linesRenderer);
        //             _pointsContainer.AddRange(_points);
        //             _pointsContainer.AddConnections(Enumerable.Range(0, N).Select(i => (i * 2, rand.Next(N) * 2)).ToArray());
        //             _pointsContainer.Remove(_points.Where(p => p.Id % 6 == 0).ToArray());
        //         }
        //
        //         [Benchmark]
        //         public void ClearPoints()
        //         {
        //             _pointsContainer.Clear();
        //         }
    }
}