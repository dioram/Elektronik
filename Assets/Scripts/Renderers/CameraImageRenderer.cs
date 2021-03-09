using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Renderers
{
    public class CameraImageRenderer : MonoBehaviour, IDataRenderer<byte[]>
    {
        public RawImage Target;
        
        #region Unity events

        private void Start()
        {
            Target.texture = Texture2D.whiteTexture;
        }
        
        private void Update()
        {
            if (_clearImage)
            {
                _clearImage = false;
                _imageChanged = false;
                Target.texture = Texture2D.whiteTexture;
                return;
            }
            
            if (!_imageChanged) return;
            _imageChanged = false;

            try
            {
                _sync.EnterReadLock();
                Texture2D texture2D = Texture2D.blackTexture;
                texture2D.LoadImage(_array);
                Target.texture = texture2D;
            }
            finally
            {
                _sync.ExitReadLock();
            }
        }

        #endregion

        #region IDataRenderer
        
        public void Render(byte[] array)
        {
            try
            {
                _sync.EnterWriteLock();
                _array = array;
                _imageChanged = true;
            }
            finally
            {
                _sync.ExitWriteLock();
            }
        }

        public void Clear()
        {
            _clearImage = true;
        }

        #endregion

        #region Private definitions

        private bool _imageChanged;
        private bool _clearImage;
        private byte[] _array;
        private readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();

        #endregion
    }
}