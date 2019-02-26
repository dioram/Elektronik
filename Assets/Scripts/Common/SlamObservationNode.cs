using Elektronik.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Common
{
    public class SlamObservationNode
    {
        // представление узла в сцене
        public GameObject ObservationObject { get; private set; }
        public SlamObservation SlamObservation { get; set; }

        public Dictionary<SlamObservationNode, int> NodeLineIDPair { get; private set; }

        public SlamObservationNode(GameObject gameObject)
        {
            NodeLineIDPair = new Dictionary<SlamObservationNode, int>();
            ObservationObject = gameObject;
        }
    }
}
