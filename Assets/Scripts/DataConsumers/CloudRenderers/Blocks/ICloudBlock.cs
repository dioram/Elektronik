using System;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public interface ICloudBlock<TGpuItem> : IDisposable
    {
        float Scale { get; set; }
        
        TGpuItem this[int index] { get; set; }
        
        int RenderQueue { get; }

        void UpdateDataOnGPU();

        void RenderData();
    }
}