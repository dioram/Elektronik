using System;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Interface for block of cloud items for rendering. </summary>
    /// <remarks> It is useful because we do not want to update full cloud when one item was updates. </remarks>
    /// <typeparam name="TGpuItem"> Type of data that will be sent on GPU. </typeparam>
    internal interface ICloudBlock<TGpuItem> : IDisposable
    {
        /// <summary> Scene scale. </summary>
        float Scale { get; set; }

        TGpuItem this[int index] { get; set; }

        /// <summary> Order in render queue. More is later. </summary>
        int RenderQueue { get; }

        /// <summary> Sends data to GPU. Should be called in MainThread and in MonoBehaviour.Update(). </summary>
        void UpdateDataOnGpu();

        /// <summary>
        /// Renders data on GPU. Should be called in MainThread and in MonoBehaviour.OnRenderObject().
        /// </summary>
        void RenderData();
    }
}