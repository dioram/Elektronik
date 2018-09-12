using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elektronik.Offline
{
    public class SlamObservationNode
    {
        // представление узла в сцене
        public GameObject ObservationObject { get; private set; }
        public SlamObservation SlamObservation { get; set; }

        public Dictionary<SlamObservationNode, int> NodeLineIDPair { get; private set; }
        public Vector3 Position { get; set; }

        public SlamObservationNode(GameObject gameObject)
        {
            NodeLineIDPair = new Dictionary<SlamObservationNode, int>();
            ObservationObject = gameObject;
        }
    }
}
