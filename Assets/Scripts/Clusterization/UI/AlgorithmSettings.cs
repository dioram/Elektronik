using Elektronik.Clusterization.Algorithms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Elektronik.Clusterization.UI
{
    public class AlgorithmSettings<TAlgorithm> : MonoBehaviour
            where TAlgorithm : IClusterizationAlgorithm
    {
        public Button ComputeButton;
        public UnityEvent<TAlgorithm> OnComputePressed;
    }
}