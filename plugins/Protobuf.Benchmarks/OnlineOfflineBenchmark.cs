using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Elektronik;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using Elektronik.Extensions;
using Elektronik.Offline;
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
        public void Offline()
        {
            var points = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                    new SlamLinesContainer());
            var observations =
                    new ConnectableObjectsContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                     new SlamLinesContainer());
            var infinitePlanes = new CloudContainer<SlamInfinitePlane>();
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
        public void Online()
        {
            var points = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                    new SlamLinesContainer());
            var observations =
                    new ConnectableObjectsContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                     new SlamLinesContainer());
            var infinitePlanes = new CloudContainer<SlamInfinitePlane>();
            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new PointsMapManager(points, new FakeConverter(), new FakeLogger()),
                new ObservationsMapManager(observations, new FakeConverter(), new FakeLogger()),
                new InfinitePlanesMapManager(infinitePlanes, new FakeConverter(), new FakeLogger())
            }.BuildChain();

            int i = 0;
            foreach (var packet in _packets)
            {
                var response = servicesChain.Handle(packet, null).Result;
                if (response.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded) i++;
            }
        }

        [Benchmark]
        public void OnlineBuffered()
        {
            var points = new ConnectableObjectsContainer<SlamPoint>(new CloudContainer<SlamPoint>(),
                                                                    new SlamLinesContainer());
            var observations =
                    new ConnectableObjectsContainer<SlamObservation>(new CloudContainer<SlamObservation>(),
                                                                     new SlamLinesContainer());
            var infinitePlanes = new CloudContainer<SlamInfinitePlane>();
            var buffer = new UpdatableFramesCollection<ICommand>();
            buffer.FramesAmountChanged += _ =>
            {
                if (buffer.MoveNext()) buffer.Current!.Execute();
            };
            var servicesChain = new IChainable<MapsManagerPb.MapsManagerPbBase>[]
            {
                new Elektronik.Protobuf.OnlineBuffered.GrpcServices.PointsMapManager(
                    buffer, points, new FakeConverter(), new FakeLogger()),
                new Elektronik.Protobuf.OnlineBuffered.GrpcServices.ObservationsMapManager(
                    buffer, observations, new FakeConverter(), new FakeLogger()),
                new Elektronik.Protobuf.OnlineBuffered.GrpcServices.InfinitePlanesMapManager(
                    buffer, infinitePlanes, new FakeConverter(), new FakeLogger())
            }.BuildChain();

            int i = 0;
            foreach (var packet in _packets)
            {
                var response = servicesChain.Handle(packet, null).Result;
                if (response.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded) i++;
            }
        }

        // Before
        // |         Method |     Mean |    Error |   StdDev |     Gen 0 |     Gen 1 |    Gen 2 | Allocated |
        // |--------------- |---------:|---------:|---------:|----------:|----------:|---------:|----------:|
        // |        Offline | 75.31 ms | 1.348 ms | 2.431 ms | 6750.0000 | 2125.0000 | 500.0000 |     38 MB |
        // |         Online | 68.17 ms | 1.284 ms | 1.138 ms | 6000.0000 | 1500.0000 | 375.0000 |     33 MB |
        // | OnlineBuffered | 75.27 ms | 0.862 ms | 0.764 ms | 7000.0000 | 2125.0000 | 500.0000 |     39 MB |


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