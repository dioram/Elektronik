using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.Cameras
{
    [RequireComponent(typeof(RawImage))]
    public class CameraImageRenderer : MonoBehaviour
    {
        private RawImage _target;
        private bool _imageChanged;
        private bool _clearImage;
        private byte[] _array;
        private readonly ReaderWriterLockSlim _sync = new ReaderWriterLockSlim();

        private void Start()
        {
            _target = GetComponent<RawImage>();
            _target.texture = Texture2D.whiteTexture;
        }
        
        private void Update()
        {
            if (_clearImage)
            {
                _clearImage = false;
                _imageChanged = false;
                _target.texture = Texture2D.whiteTexture;
                return;
            }
            
            if (!_imageChanged) return;
            _imageChanged = false;

            try
            {
                _sync.EnterReadLock();
                Texture2D texture2D = Texture2D.blackTexture;
                texture2D.LoadImage(_array);
                _target.texture = texture2D;
            }
            finally
            {
                _sync.ExitReadLock();
            }
        }
        
        public void DrawImage(byte[] array)
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

        public void ClearImage()
        {
            _clearImage = true;
        }
    }
}