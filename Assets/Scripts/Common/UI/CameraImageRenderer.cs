using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Common.Cameras
{
    [RequireComponent(typeof(RawImage))]
    public class CameraImageRenderer : MonoBehaviour
    {
        private RawImage m_target;
        private bool m_imageChanged;
        private int m_width;
        private int m_height;
        private bool m_clearImage;
        private byte[] m_array;
        private ReaderWriterLockSlim Sync = new ReaderWriterLockSlim();

        private void Start()
        {
            m_target = GetComponent<RawImage>();
            m_target.texture = Texture2D.whiteTexture;
        }
        
        private void Update()
        {
            if (m_clearImage)
            {
                m_clearImage = false;
                m_imageChanged = false;
                m_target.texture = Texture2D.whiteTexture;
                return;
            }
            
            if (!m_imageChanged) return;
            m_imageChanged = false;

            try
            {
                Sync.EnterReadLock();
                Texture2D texture2D = Texture2D.blackTexture;
                texture2D.LoadImage(m_array);
                m_target.texture = texture2D;
            }
            finally
            {
                Sync.ExitReadLock();
            }
        }
        
        public void DrawImage(byte[] array)
        {
            try
            {
                Sync.EnterWriteLock();
                m_array = array;
                m_imageChanged = true;
            }
            finally
            {
                Sync.ExitWriteLock();
            }
        }

        public void ClearImage()
        {
            m_clearImage = true;
        }
    }
}