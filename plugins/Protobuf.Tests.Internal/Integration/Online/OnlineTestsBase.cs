using System.Linq;
using System.Threading;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.Online;
using Elektronik.Renderers;
using FluentAssertions;
using Grpc.Core;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.Online
{
    public class OnlineTestsBase
    {
        protected readonly MapsManagerPb.MapsManagerPbClient MapClient;
        protected readonly ImageManagerPb.ImageManagerPbClient ImageClient;
        protected readonly SceneManagerPb.SceneManagerPbClient SceneClient;

        protected readonly ProtobufOnlinePlayer Sut;
        protected readonly Mock<ICloudRenderer<SlamPoint>> MockedPointsRenderer;
        protected readonly Mock<ICloudRenderer<SlamLine>> MockedSlamLinesRenderer;
        protected readonly Mock<ICloudRenderer<SimpleLine>> MockedSimpleLinesRenderer;
        protected readonly Mock<ICloudRenderer<SlamObservation>> MockedObservationsRenderer;
        protected readonly Mock<ICloudRenderer<SlamTrackedObject>> MockedTrackedObjsRenderer;
        protected readonly Mock<ICloudRenderer<SlamInfinitePlane>> MockedInfinitePlanesRenderer;
        protected readonly Mock<IDataRenderer<byte[]>> MockedImageRenderer;

        protected OnlineTestsBase(int port)
        {
            var f = new ProtobufOnlinePlayerFactory()
                    { Settings = new OnlineSettingsBag { ListeningPort = port }, Logger = new TestsLogger() };
            Sut = (ProtobufOnlinePlayer)f.Start(new FakeConverter());
            Sut.Play();

            var channel = new Channel($"127.0.0.1:{port}", ChannelCredentials.Insecure);
            MapClient = new MapsManagerPb.MapsManagerPbClient(channel);
            ImageClient = new ImageManagerPb.ImageManagerPbClient(channel);
            SceneClient = new SceneManagerPb.SceneManagerPbClient(channel);

            MockedPointsRenderer = new Mock<ICloudRenderer<SlamPoint>>();
            MockedSlamLinesRenderer = new Mock<ICloudRenderer<SlamLine>>();
            MockedSimpleLinesRenderer = new Mock<ICloudRenderer<SimpleLine>>();
            MockedObservationsRenderer = new Mock<ICloudRenderer<SlamObservation>>();
            MockedTrackedObjsRenderer = new Mock<ICloudRenderer<SlamTrackedObject>>();
            MockedInfinitePlanesRenderer = new Mock<ICloudRenderer<SlamInfinitePlane>>();
            MockedImageRenderer = new Mock<IDataRenderer<byte[]>>();

            SetupPrintFromMocks();

            Sut.Data.AddRenderer(MockedPointsRenderer.Object);
            Sut.Data.AddRenderer(MockedSlamLinesRenderer.Object);
            Sut.Data.AddRenderer(MockedSimpleLinesRenderer.Object);
            Sut.Data.AddRenderer(MockedObservationsRenderer.Object);
            Sut.Data.AddRenderer(MockedTrackedObjsRenderer.Object);
            Sut.Data.AddRenderer(MockedInfinitePlanesRenderer.Object);
            Sut.Data.AddRenderer(MockedImageRenderer.Object);
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
        }

        private string ToString(SlamPoint p) => $"{p.Id}, {p.Position}, {p.Color}, \"{p.Message}\"";

        private string ToString(SlamTrackedObject p) =>
                $"{p.Id}, {p.Position}, {p.Rotation}, {p.Color}, \"{p.Message}\"";

        private string ToString(SimpleLine p) => $"{p.Id}, {p.BeginPos}, {p.EndPos}, {p.BeginColor}, {p.EndColor}";
    }
}