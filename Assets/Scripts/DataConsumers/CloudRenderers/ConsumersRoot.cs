using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class ConsumersRoot : MonoBehaviour
    {
        public List<IDataConsumer> GetConsumers()
        {
            return GetComponentsInChildren<IDataConsumer>()
                    .Where(c => ((MonoBehaviour)c).transform.parent == transform)
                    .ToList();
        }
    }
}