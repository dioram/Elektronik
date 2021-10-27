using System.Linq;
using System.Threading;
using Elektronik.DataConsumers;
using Elektronik.DataObjects;
using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Plugins.Common;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online;
using FluentAssertions;
using Grpc.Core;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    public class OnlineTestsBase
    {
        protected readonly MapsManagerPb.MapsManagerPbClient MapClient;
        protected readonly SceneManagerPb.SceneManagerPbClient SceneClient;

        protected readonly ProtobufOnlinePlayer Sut;
        protected readonly Mock<ICloudRenderer<SlamPoint>> MockedPointsRenderer;
        protected readonly Mock<ICloudRenderer<SlamLine>> MockedSlamLinesRenderer;
        protected readonly Mock<ICloudRenderer<SimpleLine>> MockedSimpleLinesRenderer;
        protected readonly Mock<ICloudRenderer<SlamObservation>> MockedObservationsRenderer;
        protected readonly Mock<ICloudRenderer<SlamTrackedObject>> MockedTrackedObjsRenderer;
        protected readonly Mock<ICloudRenderer<SlamPlane>> MockedPlanesRenderer;
        protected readonly Mock<ICloudRenderer<SlamMarker>> MockedMarkerRenderer;
        protected readonly Mock<IDataRenderer<byte[]>> MockedImageRenderer;
        protected readonly ICSConverter Converter = new ProtobufToUnityConverter();

        protected OnlineTestsBase(int port)
        {
            var f = new ProtobufOnlinePlayerFactory()
                    { Settings = new OnlineSettingsBag { ListeningPort = port }, Logger = new TestsLogger() };
            Sut = (ProtobufOnlinePlayer)f.Start();
            Sut.Play();

            var channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
            MapClient = new MapsManagerPb.MapsManagerPbClient(channel);
            SceneClient = new SceneManagerPb.SceneManagerPbClient(channel);

            MockedPointsRenderer = new Mock<ICloudRenderer<SlamPoint>>();
            MockedSlamLinesRenderer = new Mock<ICloudRenderer<SlamLine>>();
            MockedSimpleLinesRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            MockedObservationsRenderer = new Mock<ICloudRenderer<SlamObservation>>();
            MockedTrackedObjsRenderer = new Mock<ICloudRenderer<SlamTrackedObject>>();
            MockedPlanesRenderer = new Mock<ICloudRenderer<SlamPlane>>();
            MockedMarkerRenderer = new Mock<ICloudRenderer<SlamMarker>>();
            MockedImageRenderer = new Mock<IDataRenderer<byte[]>>();

            SetupPrintFromMocks();

            Sut.Data.AddConsumer(MockedPointsRenderer.Object);
            Sut.Data.AddConsumer(MockedSlamLinesRenderer.Object);
            Sut.Data.AddConsumer(MockedSimpleLinesRenderer.Object);
            Sut.Data.AddConsumer(MockedObservationsRenderer.Object);
            Sut.Data.AddConsumer(MockedTrackedObjsRenderer.Object);
            Sut.Data.AddConsumer(MockedPlanesRenderer.Object);
            Sut.Data.AddConsumer(MockedImageRenderer.Object);
            Sut.Data.AddConsumer(MockedMarkerRenderer.Object);
        }

        protected void SendPacket(PacketPb packet)
        {
            var response = MapClient.Handle(packet);
            Thread.Sleep(40);
            response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
        }

        private void SetupPrintFromMocks()
        {
            MockedPointsRenderer
                    .Setup(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamPoint>>()))
                    .Callback((object sender, AddedEventArgs<SlamPoint> e) =>
                    {
                        var ss = e.AddedItems.Select(ToString);
                        TestContext.WriteLine($"Points added: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedPointsRenderer
                    .Setup(r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SlamPoint>>()))
                    .Callback((object sender, UpdatedEventArgs<SlamPoint> e) =>
                    {
                        var ss = e.UpdatedItems.Select(ToString);
                        TestContext.WriteLine($"Points updated: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedPointsRenderer
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs<SlamPoint>>()))
                    .Callback((object sender, RemovedEventArgs<SlamPoint> e) =>
                    {
                        TestContext.WriteLine(
                            $"Points removed: {sender}\n{string.Join(", ", e.RemovedItems.Select(i => i.Id))}");
                    });


            MockedSlamLinesRenderer
                    .Setup(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamLine>>()))
                    .Callback((object sender, AddedEventArgs<SlamLine> e) =>
                    {
                        var ss = e.AddedItems
                                .Select(l => $"{l.Id}, \"{l.Message}\", " +
                                                $"[{ToString(l.Point1)} | " +
                                                $"{ToString(l.Point2)}]");
                        TestContext.WriteLine($"Lines added: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedSlamLinesRenderer
                    .Setup(r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SlamLine>>()))
                    .Callback((object sender, UpdatedEventArgs<SlamLine> e) =>
                    {
                        var ss = e.UpdatedItems
                                .Select(l => $"{l.Id}, \"{l.Message}\", " +
                                                $"[{ToString(l.Point1)} | " +
                                                $"{ToString(l.Point2)}]");
                        TestContext.WriteLine($"Lines updated: {sender}\n{string.Join(", ", ss)}");
                    });
            MockedSlamLinesRenderer
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs<SlamLine>>()))
                    .Callback((object sender, RemovedEventArgs<SlamLine> e) =>
                    {
                        TestContext.WriteLine(
                            $"Slam lines removed: {sender}\n{string.Join(", ", e.RemovedItems.Select(l => $"({l.Point1.Id}, {l.Point2.Id})"))}");
                    });


            MockedObservationsRenderer
                    .Setup(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamObservation>>()))
                    .Callback((object sender, AddedEventArgs<SlamObservation> e) =>
                    {
                        var ss = e.AddedItems
                                .Select(o => $"{o.Id}, [{ToString(o.Point)}], {o.Rotation}, " +
                                                $"{o.ObservedPoints.Count}, \"{o.Message}\", \"{o.FileName}\"");
                        TestContext.WriteLine($"Observations added: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedObservationsRenderer
                    .Setup(r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SlamObservation>>()))
                    .Callback((object sender, UpdatedEventArgs<SlamObservation> e) =>
                    {
                        var ss = e.UpdatedItems
                                .Select(o => $"{o.Id}, [{ToString(o.Point)}], {o.Rotation}, " +
                                                $"{o.ObservedPoints.Count}, \"{o.Message}\", \"{o.FileName}\"");
                        TestContext.WriteLine($"Observations updated: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedObservationsRenderer
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs<SlamObservation>>()))
                    .Callback((object sender, RemovedEventArgs<SlamObservation> e) =>
                    {
                        TestContext.WriteLine(
                            $"Observations removed: {sender}\n{string.Join(", ", e.RemovedItems.Select(i => i.Id))}");
                    });


            MockedTrackedObjsRenderer
                    .Setup(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamTrackedObject>>()))
                    .Callback((object sender, AddedEventArgs<SlamTrackedObject> e) =>
                    {
                        var ss = e.AddedItems.Select(ToString);
                        TestContext.WriteLine($"Tracked objects added: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedTrackedObjsRenderer
                    .Setup(r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SlamTrackedObject>>()))
                    .Callback((object sender, UpdatedEventArgs<SlamTrackedObject> e) =>
                    {
                        var ss = e.UpdatedItems.Select(ToString);
                        TestContext.WriteLine($"Tracked objects updated: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedTrackedObjsRenderer
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs<SlamTrackedObject>>()))
                    .Callback((object sender, RemovedEventArgs<SlamTrackedObject> e) =>
                    {
                        TestContext.WriteLine(
                            $"Tracked objects removed: {sender}\n{string.Join(", ", e.RemovedItems.Select(i => i.Id))}");
                    });


            MockedSimpleLinesRenderer
                    .Setup(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SimpleLine>>()))
                    .Callback((object sender, AddedEventArgs<SimpleLine> e) =>
                    {
                        var ss = e.AddedItems.Select(ToString);
                        TestContext.WriteLine($"Simple lines added: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedSimpleLinesRenderer
                    .Setup(r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SimpleLine>>()))
                    .Callback((object sender, UpdatedEventArgs<SimpleLine> e) =>
                    {
                        var ss = e.UpdatedItems.Select(ToString);
                        TestContext.WriteLine($"Simple lines updated: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedSimpleLinesRenderer
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs<SimpleLine>>()))
                    .Callback((object sender, RemovedEventArgs<SimpleLine> e) =>
                    {
                        TestContext.WriteLine(
                            $"Simple lines removed: {sender}\n{string.Join(", ", e.RemovedItems.Select(i => i.Id))}");
                    });


            MockedMarkerRenderer
                    .Setup(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamMarker>>()))
                    .Callback((object sender, AddedEventArgs<SlamMarker> e) =>
                    {
                        var ss = e.AddedItems
                                .Select(ToString);
                        TestContext.WriteLine($"Markers added: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedMarkerRenderer
                    .Setup(r => r.OnItemsUpdated(It.IsAny<object>(), It.IsAny<UpdatedEventArgs<SlamMarker>>()))
                    .Callback((object sender, UpdatedEventArgs<SlamMarker> e) =>
                    {
                        var ss = e.UpdatedItems
                                .Select(ToString);
                        TestContext.WriteLine($"Markers updated: {sender}\n{string.Join("\n", ss)}");
                    });
            MockedMarkerRenderer
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs<SlamMarker>>()))
                    .Callback((object sender, RemovedEventArgs<SlamMarker> e) =>
                    {
                        TestContext.WriteLine(
                            $"Markers removed: {sender}\n{string.Join(", ", e.RemovedItems.Select(i => i.Id))}");
                    });
        }

        private string ToString(SlamPoint p) => $"{p.Id}, {p.Position}, {p.Color}, \"{p.Message}\"";

        private string ToString(SlamTrackedObject p) =>
                $"{p.Id}, {p.Position}, {p.Rotation}, {p.Color}, \"{p.Message}\"";

        private string ToString(SimpleLine p) => $"{p.Id}, {p.BeginPos}, {p.EndPos}, {p.BeginColor}, {p.EndColor}";

        private string ToString(SlamMarker m) =>
                $"{m.Id}, {m.Type}, {m.Position}, {m.Rotation}, {m.Scale}, \"{m.Message}\"";
    }
}