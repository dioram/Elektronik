using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Renderers
{
    public class ImageRenderer : MonoBehaviour, IDataRenderer<byte[]>,
                                 IDataRenderer<(int width, int height, byte[] array, TextureFormat format)>
    {
        public bool FlipVertically = false;

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

        public bool IsShowing { get; private set; }

        public void Render(byte[] array)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                Texture2D texture2D = Texture2D.blackTexture;
                texture2D.LoadImage(array);
                Fitter.aspectRatio = texture2D.width / (float)texture2D.height;
                Target.texture = texture2D;
            });
        }

        public void Render((int width, int height, byte[] array, TextureFormat format) data)
        {
            MainThreadInvoker.Instance.Enqueue(() =>
            {
                if (_texture == null 
                    || _texture.width != data.width 
                    || _texture.height != data.height 
                    || _texture.format != data.format)
                {
                    _texture = new Texture2D(data.width, data.height, data.format, false);
                }
                _texture.LoadRawTextureData(data.array);
                if (FlipVertically) FlipTextureVertically(_texture);
                _texture.Apply();
                Fitter.aspectRatio = data.width / (float)data.height;
                Target.texture = _texture;
            });
        }

        public void Clear()
        {
            MainThreadInvoker.Instance.Enqueue(() => Target.texture = Texture2D.whiteTexture);
        }

        #endregion

        #region Private
        
        [SerializeField] private RawImage Target;
        [SerializeField] private AspectRatioFitter Fitter;
        private Texture2D _texture;

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
                    newPixels[x + y * width] = originalPixels[x + (rows - y -1) * width];
                }
            }

            original.SetPixels(newPixels);
        }

        #endregion
    }
}