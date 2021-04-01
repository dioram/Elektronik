using Elektronik.Renderers;
using Elektronik.RosPlugin.Common.RosMessages;
using Elektronik.RosPlugin.Ros2.Bag.Data;
using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using SQLite;
using UnityEngine;

namespace Elektronik.RosPlugin.Ros2.Bag.Containers
{
    public class ImageDBContainer 
            : DBContainerToWindow<Image, ImageRenderer, (int width, int height, byte[] array, TextureFormat format)>
    {

        public ImageDBContainer(string displayName, SQLiteConnection dbModel, Topic topic, long[] actualTimestamps) 
                : base(displayName, dbModel, topic, actualTimestamps)
        {
        }
        
        public override bool IsActive
        {
            get => Renderer is not null && Renderer.IsShowing;
            set { }
        }

        #region DBContainerToWindow
        
        protected override (int width, int height, byte[] array, TextureFormat format) ToRenderType(Image message)
        {
            return ((int) message.width, (int) message.height, message.data,
                    RosMessageConvertExtender.GetTextureFormat(message.encoding));
        }
        
        protected override void SetRendererCallback()
        {
            if (Renderer is not null) Renderer.FlipVertically = true;
        }

        #endregion

    }
}