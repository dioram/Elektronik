using Elektronik.DataSources.Containers.EventArgs;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Wrapper for GPU mesh renderer. Use it to add renderer on Unity scene. </summary>
    internal class GpuMeshRendererComponent : MonoBehaviour, IMeshRenderer, IGpuRenderer
    {
        #region Editor fields

        /// <summary> Shaders that can be used to render mesh. </summary>
        [SerializeField] [Tooltip("Shaders that can be used to render mesh.")]
        private Shader[] Shaders;

        #endregion

        #region IMeshRenderer

        /// <inheritdoc />
        public int ShaderId
        {
            get => _nestedRenderer?.ShaderId ?? 0;
            set => _nestedRenderer.ShaderId = value;
        }

        /// <inheritdoc cref="IScalable" />
        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _nestedRenderer.Scale = value;
            }
        }

        /// <inheritdoc />
        public void OnMeshUpdated(object sender, MeshUpdatedEventArgs e)
        {
            _nestedRenderer?.OnMeshUpdated(sender, e);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _nestedRenderer.Dispose();
        }

        #endregion

        #region IGpuRenderer

        /// <inheritdoc />
        public void UpdateDataOnGpu()
        {
            _nestedRenderer?.UpdateDataOnGpu();
        }

        /// <inheritdoc />
        public void RenderNow()
        {
            _nestedRenderer?.RenderNow();
        }

        /// <inheritdoc />
        public int RenderQueue => _nestedRenderer?.RenderQueue ?? 0;

        #endregion

        #region Unity events

        private void Start()
        {
            _nestedRenderer = new GpuMeshRenderer(Shaders, Scale);
        }

        private void Update()
        {
            _nestedRenderer?.UpdateDataOnGpu();
        }

        private void OnDestroy()
        {
            _nestedRenderer.Dispose();
        }

        #endregion

        #region Private

        private float _scale = 1;
        private GpuMeshRenderer _nestedRenderer;

        #endregion
    }
}