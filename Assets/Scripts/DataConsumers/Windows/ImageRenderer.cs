using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.DataConsumers.Windows
{
    /// <summary> Renders images to window. </summary>
    public class ImageRenderer : MonoBehaviour, IDataRenderer<byte[]>, IDataRenderer<ImageData?>
    {
        #region Editor fields

        /// <summary> Render target for image. </summary>
        [SerializeField] [Tooltip("Render target for image.")]
        private RawImage Target;

        /// <summary> Component that changes UI rect of target to fit image. </summary>
        [SerializeField] [Tooltip("Component that changes UI rect to fit image.")]
        private AspectRatioFitter Fitter;

        /// <summary> Placeholder for not supported type of image. </summary>
        [SerializeField] [Tooltip("Placeholder for not supported type of image.")]
        private Texture2D NotSupportedPlaceholder;

        #endregion

        #region Unity events

        private void Start()
        {
            Target.texture = Texture2D.whiteTexture;
        }

        private void OnEnable()
        {
            IsShowing = true;
        }

        private void OnDisable()
        {
            IsShowing = false;
        }

        #endregion

        #region IDataRenderer

        /// <inheritdoc cref="IDataRenderer{T}.IsShowing" />
        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                if (_isShowing == value) return;
                _isShowing = value;
                UniRxExtensions.StartOnMainThread(() => gameObject.SetActive(_isShowing)).Subscribe();
            }
        }

        /// <inheritdoc />
        public void Render(byte[] array)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                var texture2D = Texture2D.blackTexture;
                texture2D.LoadImage(array);
                texture2D.filterMode = FilterMode.Trilinear;
                Fitter.aspectRatio = texture2D.width / (float)texture2D.height;
                Target.texture = texture2D;
            }).Subscribe();
        }

        /// <inheritdoc />
        public void Render(ImageData? data)
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                if (!data.HasValue)
                {
                    Fitter.aspectRatio = NotSupportedPlaceholder.width / (float)NotSupportedPlaceholder.height;
                    Target.texture = NotSupportedPlaceholder;
                    return;
                }

                if (_texture == null
                    || _texture.width != data.Value.Width
                    || _texture.height != data.Value.Height
                    || _texture.format != data.Value.Encoding)
                {
                    _texture = new Texture2D(data.Value.Width, data.Value.Height, data.Value.Encoding, false);
                }

                _texture.LoadRawTextureData(data.Value.Data);
                if (data.Value.IsFlippedVertically) FlipTextureVertically(_texture);
                _texture.Apply();
                Fitter.aspectRatio = data.Value.Width / (float)data.Value.Height;
                Target.texture = _texture;
            }).Subscribe();
        }

        /// <inheritdoc cref="IDataRenderer{T}.Clear" />
        public void Clear()
        {
            UniRxExtensions.StartOnMainThread(() =>
            {
                if (Target != null) Target.texture = Texture2D.whiteTexture;
            }).Subscribe();
        }

        #endregion

        #region Private

        private Texture2D _texture;
        private bool _isShowing;

        private static void FlipTextureVertically(Texture2D original)
        {
            // TODO: May be i can flip image on scene instead of iterating over texture
            var originalPixels = original.GetPixels();

            var newPixels = new Color[originalPixels.Length];

            var width = original.width;
            var rows = original.height;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
                }
            }

            original.SetPixels(newPixels);
        }

        #endregion
    }
}