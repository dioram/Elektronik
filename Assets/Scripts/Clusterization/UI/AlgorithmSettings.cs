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
        public UnityEvent<MonoBehaviour, TAlgorithm> OnComputePressed;

        private void Awake()
        {
            OnComputePressed.AddListener(((arg0, algorithm) => enabled = false));
        }

        private void OnEnable()
        {
            ComputeButton.interactable = true;
        }

        private void OnDisable()
        {
            ComputeButton.interactable = false;
        }
    }
}