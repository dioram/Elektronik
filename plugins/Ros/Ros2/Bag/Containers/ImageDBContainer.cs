using System.Collections.Generic;
using Elektronik.DataConsumers.Windows;
using Elektronik.RosPlugin.Common.Containers;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class ImageDBContainer 
            : DBContainerToWindow<Image, ImageRenderer, ImageData>
    {

        public ImageDBContainer(string displayName, List<SQLiteConnection> dbModels, Topic topic,
                                List<long> actualTimestamps)
                : base(displayName, dbModels, topic, actualTimestamps)
        {
        }
        
        public override bool IsVisible
        {
            get => Renderer is not null && Renderer.IsShowing;
            set { }
        }

        #region DBContainerToWindow
        
        protected override ImageData ToRenderType(Image message)
        {
            return ImageDataExt.FromImageMessage(message);
        }
        
        protected override void SetRendererCallback()
        {
            if (Renderer is not null) Renderer.FlipVertically = true;
        }

        #endregion
    }
}