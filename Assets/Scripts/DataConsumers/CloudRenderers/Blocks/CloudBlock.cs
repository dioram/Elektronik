﻿using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public abstract class CloudBlock : MonoBehaviour, ICloudBlock
    {
        public const int Capacity = 256 * 256;
        public Shader CloudShader;
        public bool Updated { get; set; }
        public float ItemSize { get; set; } = 1f;
        public int ItemsCount { get; set; }  = 0;

        public abstract GPUItem[] GetItems();

        public virtual void Clear()
        {
            Updated = true;
        }

        public virtual void SetScale(float value)
        {
            _renderMaterial.SetFloat(_scaleShaderProp, value);
        }
        
        #region Unity events

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Start()
        {
            _renderMaterial = new Material(CloudShader) {hideFlags = HideFlags.DontSave};
            _renderMaterial.EnableKeyword("_COMPUTE_BUFFER");
        }

        protected virtual void Update()
        {
            lock (this)
            {
                if (!Updated) return;
            
                OnUpdated();

                Updated = false;
            }
        }

        protected virtual void OnRenderObject()
        {
            if (ItemsCount <= 0) return;
            _renderMaterial.SetPass(0);
            _renderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            SendData(_renderMaterial);
            Draw();
        }

        private void OnDestroy()
        {
            ReleaseBuffers();
        }

        #endregion

        #region Protected definitions
        
        protected abstract void Init();

        protected abstract void SendData(Material renderMaterial);

        protected abstract void OnUpdated();

        protected abstract void Draw();

        protected abstract void ReleaseBuffers();

        protected void ClearArray(GPUItem[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = default;
            }
        }

        #endregion

        #region Private definitions
        
        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private readonly int _scaleShaderProp = Shader.PropertyToID("_Scale");
        private Material _renderMaterial;

        #endregion
    }
}