using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using UnityEngine;

namespace Elektronik.Containers
{
    public class TraceSettings : MonoBehaviour
    {
        public static int Duration = 2000;
        public DataSourcesController DataSourcesController;
        
        public void SetDuration(float duration)
        {
            Duration = (int) duration;
            DataSourcesController.MapSourceTree((tree, str) =>
            {
                if (tree is ITraceable traceable) traceable.TraceDuration = Duration;
            });
        }
    }
}