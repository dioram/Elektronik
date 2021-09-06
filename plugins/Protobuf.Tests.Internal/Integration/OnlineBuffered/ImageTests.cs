using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using FluentAssertions;
using Google.Protobuf;
using Moq;
using NUnit.Framework;

namespace Protobuf.Tests.Internal.Integration.OnlineBuffered
{
    [TestFixture, FixtureLifeCycle(LifeCycle.SingleInstance)]
    public class ImageTests: OnlineTestsBase
    {
        public ImageTests() : base(40014)
        {
        }

        [Test, Order(1)]
        public void SendImage()
        {
            for (int i = 1; i < 4; i++)
            {
                byte[] array = File.ReadAllBytes($"{i}.png");
                
                var packet = new ImagePacketPb
                {
                    ImageData = ByteString.CopyFrom(array, 0, array.Length),
                };

                var response = ImageClient.Handle(packet);
                response.ErrType.Should().Be(ErrorStatusPb.Types.ErrorStatusEnum.Succeeded);
                MockedImageRenderer.Verify(r => r.Render(array), Times.Once);
            }
        }

        [Test, Order(9)]
        public void CheckCalls()
        {
            ((ProtobufContainerTree)Sut.Data).Points.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Points.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).Observations.Connections.Count().Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).InfinitePlanes.Count.Should().Be(0);
            ((ProtobufContainerTree)Sut.Data).TrackedObjs.Count.Should().Be(0);
            
            MockedImageRenderer.Verify(r => r.Render(It.IsAny<byte[]>()), Times.Exactly(3));
            MockedImageRenderer.Verify(r => r.Clear(), Times.Never);
            
            
            MockedPointsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(), It.IsAny<AddedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(), 
                                                              It.IsAny<UpdatedEventArgs<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(), It.IsAny<RemovedEventArgs>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SlamPoint>>()),
                                        Times.Never);
            MockedPointsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedSlamLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<IContainer<SlamLine>>(),
                                                               It.IsAny<AddedEventArgs<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<UpdatedEventArgs<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<IContainer<SlamLine>>(),
                                                                 It.IsAny<RemovedEventArgs>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SlamLine>>()),
                                           Times.Never);
            MockedSlamLinesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedSimpleLinesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), It.IsAny<IList<SimpleLine>>()),
                                             Times.Never);
            MockedSimpleLinesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedObservationsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                  It.IsAny<AddedEventArgs<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                    It.IsAny<UpdatedEventArgs<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                    It.IsAny<RemovedEventArgs>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), 
                                                               It.IsAny<IList<SlamObservation>>()),
                                              Times.Never);
            MockedObservationsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedTrackedObjsRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                 It.IsAny<AddedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                   It.IsAny<UpdatedEventArgs<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                   It.IsAny<RemovedEventArgs>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), 
                                                              It.IsAny<IList<SlamTrackedObject>>()),
                                             Times.Never);
            MockedTrackedObjsRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);


            MockedInfinitePlanesRenderer.Verify(r => r.OnItemsAdded(It.IsAny<object>(),
                                                                    It.IsAny<AddedEventArgs<SlamInfinitePlane>>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.OnItemsUpdated(It.IsAny<object>(),
                                                                      It.IsAny<UpdatedEventArgs<SlamInfinitePlane>>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.OnItemsRemoved(It.IsAny<object>(),
                                                                      It.IsAny<RemovedEventArgs>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.ShowItems(It.IsAny<object>(), 
                                                                 It.IsAny<IList<SlamInfinitePlane>>()),
                                                Times.Never);
            MockedInfinitePlanesRenderer.Verify(r => r.OnClear(It.IsAny<object>()), Times.Never);
        }
    }
}