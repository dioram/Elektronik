using Elektronik.DataObjects;
using UnityEngine;
using MarkerGpuData = Elektronik.DataConsumers.CloudRenderers.MarkerCloudBlock.MarkerGpuData;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for specific types of markers primitives. </summary>
    /// <typeparam name="TCloudBlock"> Type of blocks this renderer is using. </typeparam>
    internal class MarkerCloudRenderer<TCloudBlock> : GpuCloudRenderer<SlamMarker, TCloudBlock, MarkerGpuData>,
                                                    IMarkerCloudRenderer
            where TCloudBlock : MarkerCloudBlock, new()
    {
        /// <summary> Constructor. </summary>
        /// <param name="shader"> Shader for rendering this block. It should handle compute buffer. </param>
        /// <param name="markerType"> Type of primitive to render. </param>
        /// <param name="scale"> Initial scale of scene. </param>
        public MarkerCloudRenderer(Shader shader, SlamMarker.MarkerType markerType, float scale) : base(shader, scale)
        {
            MarkerType = markerType;
        }

        /// <summary> Type of primitive to render. </summary>
        public SlamMarker.MarkerType MarkerType { get; }

        #region Protected

        protected override int BlockCapacity => MarkerCloudBlock.Capacity;

        protected override TCloudBlock CreateNewBlock()
        {
            var res = new TCloudBlock();
            res.InitShader(Shader);
            res.Scale = Scale;
            return res;
        }

        protected override void ProcessItem(TCloudBlock block, SlamMarker item, int inBlockId)
        {
            block[inBlockId] = new MarkerGpuData(Matrix4x4.TRS(item.Position, item.Rotation, Vector3.one),
                                                 item.Scale, item.Color);
        }

        protected override void RemoveItem(TCloudBlock block, int inBlockId)
        {
            block[inBlockId] = default;
        }

        #endregion
    }
}