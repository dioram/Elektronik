using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Elektronik;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Extensions;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Offline.Parsers;
using Elektronik.Protobuf.Online.GrpcServices;
using Protobuf.Tests.Internal;
using ICommand = Elektronik.Commands.ICommand;

namespace Protobuf.Benchmarks
{
    [Config(typeof(Config))]
    [MemoryDiagnoser]
    // [NativeMemoryProfiler]
    public class OnlineOfflineBenchmark
    {
        public class Config : ManualConfig
        {
            public Config()
            {
                this.WithOptions(ConfigOptions.DisableOptimizationsValidator);
            }
        }

        private PacketPb[] _packets;

        public OnlineOfflineBenchmark()
        {
            _packets = Enumerable.Range(0, 1000)
                    .Select(i =>
                    {
                        var packet = new PacketPb
                        {
                            Action = PacketPb.Types.ActionType.Add,
                            Points = new PacketPb.Types.Points(),
                        };
                        var points = Enumerable.Range(0, 100)
                                .Select(id => new PointPb
                                {
                                    Id = id + i * 100,
                                    Position = new Vector3Pb { X = i, Y = id, Z = 5 },
                                    Color = new ColorPb { R = id },
                                    Message = $"{id + i * 100}",
                                }).ToArray();
                        packet.Points.Data.Add(points);
                        return packet;
                    })
                    .ToArray();
        }

        [Benchmark]
        public void OfflineAdd()
        {
            var points = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                    new SlamLinesContainer());
            var observations =
                    new ConnectableObjectsContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                     new SlamLinesContainer());
            var infinitePlanes = new CloudContainer<SlamPlane>();
            var parser = new ObjectsParser(infinitePlanes, points, observations, "C:/");
            var commands = new List<ICommand?> { Capacity = _packets.Length };
            parser.SetConverter(new FakeConverter());
            foreach (var packet in _packets)
            {
                var command = parser.GetCommand(packet);
                if (command == null) return;
                command.Execute();
                commands.Add(command);
            }

            var sum = commands.Count;
        }

        [Benchmark]
        public void OnlineBufferedAdd()
        {
            var points = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                    new SlamLinesContainer());
            var observations =
                    new ConnectableObjectsContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                     new SlamLinesContainer());
            var infinitePlanes = new CloudContainer<SlamPlane>();
            var buffer = new OnlineFrameBuffer();
            buffer.FramesAmountChanged += _ =>
            {
                if (buffer.MoveNext()) buffer.Current!.Command.Execute();
            };
            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(buffer, points, new FakeConverter(), new FakeLogger()),
                new ObservationsMapManager(buffer, observations, new FakeConverter(), new FakeLogger()),
                new PlanesMapManager(buffer, infinitePlanes, new FakeConverter(), new FakeLogger())
            }.BuildChain();

            int i = 0;
            foreach (var packet in _packets)
            {
                var response = servicesChain.Handle(packet, null).Result;
                if (response.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded) i++;
            }
        }

        // IEnumerable+ToList
        // |         Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        // |--------------- |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
        // |        Offline | 75.31 ms | 1.348 ms | 2.431 ms | 6750.0000 | 2125.0000 | 500.0000 |     38 MB |
        // |         Online | 68.17 ms | 1.284 ms | 1.138 ms | 6000.0000 | 1500.0000 | 375.0000 |     33 MB |
        // | OnlineBuffered | 75.27 ms | 0.862 ms | 0.764 ms | 7000.0000 | 2125.0000 | 500.0000 |     39 MB |
        
        // IEnumerable
        // |            Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        // |------------------ |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
        // |        OfflineAdd | 71.29 ms | 1.128 ms | 1.055 ms | 5375.0000 | 2250.0000 | 625.0000 |     29 MB |
        // |         OnlineAdd | 64.95 ms | 1.116 ms | 1.044 ms | 4333.3333 | 1333.3333 | 333.3333 |     24 MB |
        // | OnlineBufferedAdd | 73.93 ms | 0.484 ms | 0.453 ms | 5500.0000 | 1875.0000 | 500.0000 |     30 MB |

        // IEnumerable-yield return
        // |            Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        // |------------------ |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
        // |        OfflineAdd | 63.78 ms | 1.144 ms | 1.014 ms | 3777.7778 | 1555.5556 | 444.4444 |     20 MB |
        // |         OnlineAdd | 51.86 ms | 0.696 ms | 0.651 ms | 2800.0000 | 1100.0000 | 200.0000 |     16 MB |
        // | OnlineBufferedAdd | 62.37 ms | 1.232 ms | 1.265 ms | 4000.0000 | 1714.2857 | 428.5714 |     21 MB |

        //IList
        // |            Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        // |------------------ |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
        // |        OfflineAdd | 60.16 ms | 0.628 ms | 0.588 ms | 3666.6667 | 1555.5556 | 444.4444 |     19 MB |
        // |         OnlineAdd | 54.77 ms | 1.013 ms | 1.244 ms | 3600.0000 | 1200.0000 | 200.0000 |     20 MB |
        // | OnlineBufferedAdd | 60.87 ms | 1.050 ms | 0.982 ms | 3888.8889 | 1666.6667 | 444.4444 |     20 MB |

        // Array
        // |            Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        // |------------------ |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
        // |        OfflineAdd | 58.99 ms | 0.981 ms | 0.918 ms | 3666.6667 | 1555.5556 | 444.4444 |     19 MB |
        // |         OnlineAdd | 51.65 ms | 0.742 ms | 0.694 ms | 3545.4545 | 1090.9091 | 181.8182 |     20 MB |
        // | OnlineBufferedAdd | 61.03 ms | 0.994 ms | 0.930 ms | 3777.7778 | 1666.6667 | 444.4444 |     20 MB |


        // * Legends *
        // Mean      : Arithmetic mean of all measurements
        // Error     : Half of 99.9% confidence interval
        // StdDev    : Standard deviation of all measurements
        // Gen 0     : GC Generation 0 collects per 1000 operations
        // Gen 1     : GC Generation 1 collects per 1000 operations
        // Gen 2     : GC Generation 2 collects per 1000 operations
        // Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
        // 1 ms      : 1 Millisecond (0.001 sec)
    }
}