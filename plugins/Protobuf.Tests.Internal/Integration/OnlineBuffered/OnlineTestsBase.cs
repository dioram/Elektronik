using System.Linq;
using Elektronik.Clouds;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Elektronik.Protobuf.OnlineBuffered;
using Elektronik.Renderers;
using Grpc.Core;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.OnlineBuffered
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

            Sut.Data.SetRenderer(MockedPointsRenderer.Object);
            Sut.Data.SetRenderer(MockedSlamLinesRenderer.Object);
            Sut.Data.SetRenderer(MockedSimpleLinesRenderer.Object);
            Sut.Data.SetRenderer(MockedObservationsRenderer.Object);
            Sut.Data.SetRenderer(MockedTrackedObjsRenderer.Object);
            Sut.Data.SetRenderer(MockedInfinitePlanesRenderer.Object);
            Sut.Data.SetRenderer(MockedImageRenderer.Object);
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
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()))
                    .Callback((object sender, RemovedEventArgs e) =>
                    {
                        TestContext.WriteLine($"Points removed: {sender}\n{string.Join(", ", e.RemovedIds)}");
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
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()))
                    .Callback((object sender, RemovedEventArgs e) =>
                    {
                        TestContext.WriteLine($"Lines removed: {sender}\n{string.Join(", ", e.RemovedIds)}");
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
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()))
                    .Callback((object sender, RemovedEventArgs e) =>
                    {
                        TestContext.WriteLine(
                            $"Observations removed: {sender}\n{string.Join(", ", e.RemovedIds)}");
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
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()))
                    .Callback((object sender, RemovedEventArgs e) =>
                    {
                        TestContext.WriteLine($"Tracked objects removed: {sender}\n{string.Join(", ", e.RemovedIds)}");
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
                    .Setup(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()))
                    .Callback((object sender, RemovedEventArgs e) =>
                    {
                        TestContext.WriteLine($"Simple lines removed: {sender}\n{string.Join(", ", e.RemovedIds)}");
                    });
        }

        private string ToString(SlamPoint p) => $"{p.Id}, {p.Position}, {p.Color}, \"{p.Message}\"";
        private string ToString(SlamTrackedObject p) => $"{p.Id}, {p.Position}, {p.Rotation}, {p.Color}, \"{p.Message}\"";
        private string ToString(SimpleLine p) => $"{p.Id}, {p.BeginPos}, {p.EndPos}, {p.BeginColor}, {p.EndColor}";
    }
}