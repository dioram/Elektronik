using Elektronik.DataObjects;
using Elektronik.DataConsumers.Windows;
using Elektronik.DataSources;
using Elektronik.DataSources.Containers;
using Elektronik.Plugins.Common.Commands;
using Elektronik.Plugins.Common.Commands.Generic;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Common.RosMessages;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace Elektronik.RosPlugin.Ros.Bag.Parsers
{
    public static class MessageToCommandExt
    {
        public static ICommand ToCommand(this Message message, IDataSource container) => message switch
        {
            PointCloud2 cloud when container is ICloudContainer<SlamPoint> pointsContainer =>
                    new MacroCommand(new ICommand[] // TODO: change to special command
                    {
                        new ClearCommand<SlamPoint>(pointsContainer),
                        new AddCommand<SlamPoint>(pointsContainer, cloud.ToSlamPoints())
                    }),
            Image image when container is ImagePresenter presenter => 
                    new ShowCommand<ImagePresenter, ImageData?>(presenter, ImageDataExt.FromImageMessage(image)),
            String str when container is StringPresenter presenter => 
                    new ShowCommand<StringPresenter, String>(presenter, str),
            _ when container is TrackedCloudObjectsContainer trackedContainer && message.GetPose() is { } pose =>
                    trackedContainer.Count switch
                    {
                        0 => new AddCommand<SlamTrackedObject>(trackedContainer, new[] {pose.ToTracked()}),
                        _ => new UpdateCommand<SlamTrackedObject>(trackedContainer, new[] {pose.ToTracked()})
                    },
            _ => new ShowCommand<UnknownTypePresenter, Message>((UnknownTypePresenter)container, message),
        };
    }
}