using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> This component gets all consumers from unity's scene tree. </summary>
    internal class ConsumersRoot : MonoBehaviour
    {
        public List<IDataConsumer> GetConsumers()
        {
            return GetComponentsInChildren<IDataConsumer>()
                    .Where(c => ((MonoBehaviour)c).transform.parent == transform)
                    .ToList();
        }
    }
}