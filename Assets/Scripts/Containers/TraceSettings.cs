using Elektronik.PluginsSystem.UnitySide;
using UnityEngine;

namespace Elektronik.Containers
{
    public class TraceSettings : MonoBehaviour
    {
        public static int Duration = 2000;
        
        public void SetDuration(float duration)
        {
            Duration = (int) duration;
            PluginsPlayer.MapSourceTree(tree =>
            {
                if (tree is ITraceable traceable) traceable.Duration = Duration;
            });
        }
    }
}