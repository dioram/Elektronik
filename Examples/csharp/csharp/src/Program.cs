using Elektronik.Common.Data.Pb;
using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace csharp
{
    class Program
    {

        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = new Channel("127.0.0.1:5050", ChannelCredentials.Insecure);
            var client = new MapsManagerPb.MapsManagerPbClient(channel);

            var tests = new ITest[]
            {
                new PointsTest(),
                new ObservationsTest(),
                new TrackedObjectsTest(),
            };

            var packets = new List<PacketPb>();
            foreach (var test in tests)
            {
                // We should keep in mind that all entities in protobuf are classes, therefore they can share data by reference.
                // To avoid data modification in PacketPb, just clone it.
                packets.AddRange(test.Create().Select(p => p.Clone()));
                packets.AddRange(test.Update().Select(p => p.Clone()));
                packets.AddRange(test.Remove().Select(p => p.Clone()));
                packets.AddRange(test.Clear().Select(p => p.Clone()));
            }

            // File.Open("tests_packets.dat", FileMode.Create) instead of File.OpenWrite because of packets should be fitted in a whole file;
            // so the file must be overwritten instead of opened.
            // Otherwise the file may contain a content which may be larger than a size of the writable packets, this can lead to a reading file error caused by a wrong stream length.
            using (var output = new BinaryWriter(File.Open("tests_packets.dat", FileMode.Create)))
            {
                foreach (var packet in packets)
                {
                    // Online usage
                    var response = client.Handle(packet);
                    if (response.ErrType != ErrorStatusPb.Types.ErrorStatusEnum.Succeeded)
                        Console.WriteLine(response.Message);

                    // Offline usage
                    // We need to write message size since file should contain sequence of messages
                    packet.Special = true;
                    packet.WriteDelimitedTo(output.BaseStream);
                    Console.WriteLine($"{packet.Action} {packet.DataCase} {packet.CalculateSize()}");
                }
            }

            Console.WriteLine();
            Console.WriteLine();

            var parsedPackets = new List<PacketPb>();
            using (var input = new BinaryReader(File.OpenRead("tests_packets.dat")))
            {
                
                while (input.BaseStream.Position != input.BaseStream.Length)
                {
                    var packet = PacketPb.Parser.ParseDelimitedFrom(input.BaseStream);
                    Console.WriteLine($"{packet.Action} {packet.DataCase} {packet.CalculateSize()}");
                }
            }
        }
    }
}
