using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> This component will call rendering of clouds in right order. </summary>
    /// <remarks>
    /// This is necessary for correct support of transparency when using <c>Graphics.DrawProceduralNow()</c>
    /// because it ignores standard unity render queue.
    /// </remarks>
    internal class RenderQueueController : MonoBehaviour
    {
        private IGpuRenderer[] _renderers;

        private void Start()
        {
            _renderers = GetComponentsInChildren<IGpuRenderer>()
                    .OrderBy(r => r.RenderQueue)
                    .ToArray();
        }

        private void OnRenderObject()
        {
            foreach (var queueableRenderer in _renderers)
            {
                queueableRenderer.RenderNow();
            }
        }
    }
}