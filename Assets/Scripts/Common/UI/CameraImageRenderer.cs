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
        private byte[] m_array;
        private ReaderWriterLockSlim Sync = new ReaderWriterLockSlim();

        private void Start()
        {
            m_target = GetComponent<RawImage>();
        }
        
        private void Update()
        {
            if (!m_imageChanged) return;
            m_imageChanged = false;

            try
            {
                Sync.EnterReadLock();
                Texture2D texture2D = new Texture2D(m_width, m_height);
                texture2D.LoadImage(m_array);
                m_target.texture = texture2D;
            }
            finally
            {
                Sync.ExitReadLock();
            }
        }
        
        public void DrawImage(int width, int height, byte[] array)
        {
            try
            {
                Sync.EnterWriteLock();
                m_array = array;
                m_width = width;
                m_height = height;
                m_imageChanged = true;
            }
            finally
            {
                Sync.ExitWriteLock();
            }
        }
    }
}