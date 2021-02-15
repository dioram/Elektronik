using System.Linq;
using UnityEngine;

namespace Elektronik.Common.Clouds.V2
{
    public abstract class CloudBlock : MonoBehaviour
    {
        public const int Capacity = 1024 * 1024;
        public Shader CloudShader;
        public bool Updated;
        public bool ToClear;
        public float ItemSize = 1f;
        
        #region Unity events

        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Start()
        {
            _renderMaterial = new Material(CloudShader);
            _renderMaterial.hideFlags = HideFlags.DontSave;
            _renderMaterial.EnableKeyword("_COMPUTE_BUFFER");
        }

        protected virtual void Update()
        {
            if (ToClear)
            {
                Clear();
                ToClear = false;
                Updated = true;
            }
            
            if (!Updated) return;
            
            OnUpdated();

            Updated = false;
        }

        protected virtual void OnRenderObject()
        {
            _renderMaterial.SetPass(0);
            _renderMaterial.SetFloat(_sizeShaderProp, ItemSize);
            SendData(_renderMaterial);
            Draw();
        }

        #endregion

        #region Protected definitions
        
        protected abstract void Init();

        protected abstract void SendData(Material renderMaterial);

        protected abstract void Clear();


        protected abstract void OnUpdated();

        protected abstract void Draw();

        #endregion

        #region Private definitions
        
        private readonly int _sizeShaderProp = Shader.PropertyToID("_Size");
        private Material _renderMaterial;

        #endregion
    }
}