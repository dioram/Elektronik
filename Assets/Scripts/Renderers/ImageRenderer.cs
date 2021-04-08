using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Renderers
{
    public class ImageRenderer : MonoBehaviour, IDataRenderer<byte[]>,
                                 IDataRenderer<ImageData>
    {
        public bool FlipVertically = false;
        public Texture2D NotSupportedPlaceholder;

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

        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                if (_isShowing == value) return;
                _isShowing = value;
                MainThreadInvoker.Instance.Enqueue(() => gameObject.SetActive(_isShowing));
            }
        }

        public void Render(byte[] array)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                Texture2D texture2D = Texture2D.blackTexture;
                texture2D.LoadImage(array);
                Fitter.aspectRatio = texture2D.width / (float) texture2D.height;
                Target.texture = texture2D;
            });
        }

        public void Render(ImageData data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                if (!data.IsSupported)
                {
                    Fitter.aspectRatio = NotSupportedPlaceholder.width / (float) NotSupportedPlaceholder.height;
                    Target.texture = NotSupportedPlaceholder;
                    return;
                }
                
                if (_texture == null
                    || _texture.width != data.Width
                    || _texture.height != data.Height
                    || _texture.format != data.Encoding)
                {
                    _texture = new Texture2D(data.Width, data.Height, data.Encoding, false);
                }

                _texture.LoadRawTextureData(data.Data);
                if (FlipVertically) FlipTextureVertically(_texture);
                _texture.Apply();
                Fitter.aspectRatio = data.Width / (float) data.Height;
                Target.texture = _texture;
            });
        }

        public void Clear()
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                if (Target != null) Target.texture = Texture2D.whiteTexture;
            });
        }

        #endregion

        #region Private

        [SerializeField] private RawImage Target;
        [SerializeField] private AspectRatioFitter Fitter;
        private Texture2D _texture;
        private bool _isShowing;

        private static void FlipTextureVertically(Texture2D original)
        {
            var originalPixels = original.GetPixels();

            Color[] newPixels = new Color[originalPixels.Length];

            int width = original.width;
            int rows = original.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
                }
            }

            original.SetPixels(newPixels);
        }

        #endregion
    }
}