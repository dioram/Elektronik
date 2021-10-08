using System;
using System.Linq;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class MarkersTests : OnlineTestsBase
    {
        private MarkerPb[] _map;

        public MarkersTests() : base(40017)
        {
            _map = Enumerable.Range(0, 5).Select(i => new MarkerPb()
            {
                Id = i,
                Position = new Vector3Pb { X = i },
                Color = new ColorPb { B = i * 50 },
                Primitive = MarkerPb.Types.Type.Crystal,
                Message = $"{i}",
            }).ToArray();
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Markers = new PacketPb.Types.Markers(),
            };
            packet.Markers.Data.Add(_map);
            var e = new AddedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(2)]
        public void UpdatePositions()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new MarkerPb(pb) { Position = { Z = i } };
                var diff = new MarkerPb { Id = i, Position = new Vector3Pb { X = i, Z = i } };
                return (newPb, diff);
            });
            packet.Markers.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(3)]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new MarkerPb(pb) { Color = new ColorPb { R = 255, G = 255, B = 255 } };
                var diff = new MarkerPb { Id = i, Color = new ColorPb { R = 255, G = 255, B = 255 } };
                return (newPb, diff);
            });
            packet.Markers.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(4)]
        public void UpdateScale()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new MarkerPb(pb) { Scale = new Vector3Pb { X = i * 0.2, Y = 10, Z = i * 0.2 } };
                var diff = new MarkerPb { Id = i, Scale = new Vector3Pb { X = i * 0.2, Y = 10, Z = i * 0.2 } };
                return (newPb, diff);
            });
            packet.Markers.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(5)]
        public void UpdateOrientation()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new MarkerPb(pb) { Orientation = new Vector4Pb { X = 1, W = 1, } };
                var diff = new MarkerPb { Id = i, Orientation = new Vector4Pb { X = 1, W = 1, } };
                return (newPb, diff);
            });
            packet.Markers.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(6)]
        public void UpdateType()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new MarkerPb(pb) { Primitive = MarkerPb.Types.Type.Cube };
                var diff = new MarkerPb { Id = i, Primitive = MarkerPb.Types.Type.Cube };
                return (newPb, diff);
            });
            packet.Markers.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(7)]
        public void ComplexUpdate()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Markers = new PacketPb.Types.Markers(),
            };
            for (var i = 0; i < _map.Length; i++)
            {
                _map[i] = new MarkerPb()
                {
                    Id = i,
                    Position = new Vector3Pb { X = i, Y = i, Z = i * 5 },
                    Color = new ColorPb { R = i * 50, G = i * 50, B = i * 50 },
                    Orientation = new Vector4Pb { Y = 1, W = 1, },
                    Scale = new Vector3Pb { X = i, Y = i, Z = i * 5 },
                    Primitive = (MarkerPb.Types.Type)i, 
                    Message = "Test message",
                };
            }

            packet.Markers.Data.Add(_map);
            var e = new UpdatedEventArgs<SlamMarker>(_map.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length);
            MockedMarkerRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(8)]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Markers = new PacketPb.Types.Markers(),
            };
            packet.Markers.Data.Add(new[] { _map[1], _map[3] });
            var e = new RemovedEventArgs<SlamMarker>(new[]
            {
                _map[1].ToUnity(Converter).Apply(),
                _map[3].ToUnity(Converter).Apply()
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(_map.Length - 2);
            MockedMarkerRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }

        [Test, Order(9)]
        public void Clear()
        {
            var packet = new PacketPb()
            {
                Action = PacketPb.Types.ActionType.Clear,
                Markers = new PacketPb.Types.Markers(),
            };
            var e = new RemovedEventArgs<SlamMarker>(new[]
            {
                _map[0].ToUnity(Converter).Apply(),
                _map[2].ToUnity(Converter).Apply(),
                _map[4].ToUnity(Converter).Apply(),
            });
            
            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Markers.Count.Should().Be(0);
            MockedMarkerRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Markers, e), Times.Once);
        }
        
        #region Not tests

        private MarkerPb[] CreateDiff(Func<MarkerPb, int, (MarkerPb, MarkerPb)> func)
        {
            var result = _map.Select(func).ToArray();
            _map = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}