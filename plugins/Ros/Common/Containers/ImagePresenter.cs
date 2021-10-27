using Elektronik.DataConsumers.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class ImagePresenter : PresenterBase<ImageData?, ImageRenderer, ImageData?>
    {
        public ImagePresenter(string displayName) : base(displayName)
        {
        }

        #region PresenterBase

        protected override void SetRendererCallback()
        {
            base.SetRendererCallback();
            if (Renderer is not null) Renderer.FlipVertically = true;
        }

        protected override ImageData? ToRenderType(ImageData? message) => message;

        #endregion
    }
}