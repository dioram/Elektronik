using Elektronik.DataConsumers.Windows;

namespace Elektronik.RosPlugin.Common.Containers
{
    public class ImagePresenter : PresenterBase<ImageData?, ImageRenderer, ImageData?>
    {
        public ImagePresenter(string displayName) : base(displayName)
        {
        }

        #region PresenterBase

        protected override ImageData? ToRenderType(ImageData? message) => message;

        #endregion
    }
}