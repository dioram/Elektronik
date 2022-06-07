﻿using System;
using System.Linq;
using System.Threading;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class PlanesTests : OnlineTestsBase
    {
        private PlanePb[] _planes;

        private readonly Vector3Pb[] _offsets =
        {
            new() { X = 0, Y = 0, Z = 0 },
            new() { X = 0, Y = 0, Z = 0 },
            new() { X = 0, Y = 0, Z = 50 },
            new() { X = 50, Y = 0, Z = 0 },
            new() { X = 0, Y = 50, Z = 0 },
        };

        private readonly Vector3Pb[] _normals =
        {
            new() { X = 0, Y = 1, Z = 0 },
            new() { X = 1, Y = 0, Z = 0 },
            new() { X = 0, Y = 0, Z = 1 },
            new() { X = 1, Y = 0, Z = 1 },
            new() { X = 0, Y = 1, Z = 0 },
        };

        public PlanesTests() : base(40013)
        {
            _planes = Enumerable.Range(0, 5).Select(i => new PlanePb
            {
                Id = i,
                Message = $"{i}",
                Color = new ColorPb { R = i * 50 },
                Normal = _normals[i],
                Offset = _offsets[i],
            }).ToArray();
        }

        [Test, Order(1)]
        public void Create()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Add,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(_planes);
            var e = new AddedEventArgs<SlamPlane>(_planes.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            var response = MapClient.Handle(packet);
            Thread.Sleep(20);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(r => r.OnItemsAdded(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(2)]
        public void UpdateOffsets()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            var newOffsets = new Vector3Pb[]
            {
                new() { X = 0, Y = 0, Z = 100 },
                new() { X = 0, Y = 100, Z = 100 },
                new() { X = 100, Y = 0, Z = 100 },
                new() { X = 0, Y = 100, Z = 100 },
                new() { X = 100, Y = 0, Z = 100 },
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PlanePb(pb) { Offset = newOffsets[i] };
                var diff = new PlanePb { Id = i, Offset = newOffsets[i] };
                return (newPb, diff);
            });
            packet.Planes.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            var response = MapClient.Handle(packet);
            Thread.Sleep(20);

            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(3)]
        public void UpdateNormals()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            var newNormals = new Vector3Pb[]
            {
                new() { X = -10, Y = -10, Z = -10 },
                new() { X = -10, Y = -10, Z = -10 },
                new() { X = -10, Y = -10, Z = -50 },
                new() { X = -50, Y = -10, Z = -10 },
                new() { X = -10, Y = -50, Z = -10 },
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PlanePb(pb) { Normal = newNormals[i] };
                var diff = new PlanePb { Id = i, Normal = newNormals[i] };
                return (newPb, diff);
            });
            packet.Planes.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(4)]
        public void UpdateColors()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            var diff = CreateDiff((pb, i) =>
            {
                var newPb = new PlanePb(pb) { Color = { R = 255, G = 255, B = 255 } };
                var diff = new PlanePb { Id = i, Color = new ColorPb { R = 255, B = 255, G = 255 } };
                return (newPb, diff);
            });
            packet.Planes.Data.Add(diff);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(5)]
        public void ComplexUpdate()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Update,
                Planes = new PacketPb.Types.Planes(),
            };
            for (var i = 0; i < _planes.Length; i++)
            {
                _planes[i] = new PlanePb
                {
                    Id = i,
                    Offset = new Vector3Pb { X = i, Y = i, Z = i },
                    Normal = new Vector3Pb { X = 1 },
                    Color = new ColorPb { R = i * 50, G = i * 50, B = i * 50 },
                    Message = "Test message",
                };
            }

            packet.Planes.Data.Add(_planes);
            var e = new UpdatedEventArgs<SlamPlane>(_planes.Select(p => p.ToUnity(Converter).Apply()).ToArray());

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length);
            MockedPlanesRenderer.Verify(r => r.OnItemsUpdated(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(6)]
        public void Remove()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Remove,
                Planes = new PacketPb.Types.Planes(),
            };
            packet.Planes.Data.Add(new[] { _planes[1], _planes[3] });
            var e = new RemovedEventArgs<SlamPlane>(new[]
            {
                _planes[1].ToUnity(Converter).Apply(),
                _planes[3].ToUnity(Converter).Apply(),
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(_planes.Length - 2);
            MockedPlanesRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        [Test, Order(7)]
        public void Clear()
        {
            var packet = new PacketPb
            {
                Action = PacketPb.Types.ActionType.Clear,
                Planes = new PacketPb.Types.Planes(),
            };
            var e = new RemovedEventArgs<SlamPlane>(new[]
            {
                _planes[0].ToUnity(Converter).Apply(),
                _planes[2].ToUnity(Converter).Apply(),
                _planes[4].ToUnity(Converter).Apply(),
            });

            SendPacket(packet);

            ((ProtobufContainerTree)Sut.Data).Planes.Count.Should().Be(0);
            MockedPlanesRenderer.Verify(r => r.OnItemsRemoved(((ProtobufContainerTree)Sut.Data).Planes, e), Times.Once);
        }

        #region Not tests

        private PlanePb[] CreateDiff(Func<PlanePb, int, (PlanePb, PlanePb)> func)
        {
            var result = _planes.Select(func).ToArray();
            _planes = result.Select(o => o.Item1).ToArray();
            return result.Select(o => o.Item2).ToArray();
        }

        #endregion
    }
}