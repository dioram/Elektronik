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
        public SlamObservation SlamObservation { get; private set; }
        public Dictionary<SlamObservationNode, int> NodeLineIDPair { get; private set; }

        public SlamObservationNode(SlamObservation slamObservation)
        {
            NodeLineIDPair = new Dictionary<SlamObservationNode, int>();
            SlamObservation = slamObservation;
        }
    }
}
