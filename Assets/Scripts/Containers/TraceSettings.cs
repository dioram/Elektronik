using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using UnityEngine;

namespace Elektronik.Containers
{
    public class TraceSettings : MonoBehaviour
    {
        public static int Duration = 2000;
        public DataSourcesManager DataSourcesManager;
        
        public void SetDuration(float duration)
        {
            Duration = (int) duration;
            DataSourcesManager.MapSourceTree((tree, str) =>
            {
                if (tree is ITraceable traceable) traceable.Duration = Duration;
            });
        }
    }
}