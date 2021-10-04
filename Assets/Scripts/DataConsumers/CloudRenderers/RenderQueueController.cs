using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class RenderQueueController : MonoBehaviour
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