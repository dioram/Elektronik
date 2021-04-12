using Elektronik.Commands;
using Elektronik.Commands.Generic;
using Elektronik.Containers;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers
{
    public static class MessageToCommandExt
    {
        public static ICommand ToCommand(this Message message, ISourceTree container) => message switch
        {
            PointCloud2 cloud when container is IContainer<SlamPoint> pointsContainer =>
                    new MacroCommand(new ICommand[]
                    {
                        new ClearCommand<SlamPoint>(pointsContainer),
                        new AddCommand<SlamPoint>(pointsContainer, cloud.ToSlamPoints())
                    }),
            Image image when container is ImagePresenter presenter => 
                    new ShowCommand<ImagePresenter, ImageData>(presenter, ImageDataExt.FromImageMessage(image)),
            String str when container is StringPresenter presenter => 
                    new ShowCommand<StringPresenter, String>(presenter, str),
            _ when container is TrackedObjectsContainer trackedContainer && message.GetPose() is { } pose =>
                    trackedContainer.Count switch
                    {
                        0 => new AddCommand<SlamTrackedObject>(trackedContainer, new[] {pose.ToTracked()}),
                        _ => new UpdateCommand<SlamTrackedObject>(trackedContainer, new[] {pose.ToTracked()})
                    },
            _ => new ShowCommand<UnknownTypePresenter, Message>((UnknownTypePresenter)container, message),
        };
    }
}