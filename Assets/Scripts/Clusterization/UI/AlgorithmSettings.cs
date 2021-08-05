using System;
using Elektronik.Clusterization.Algorithms;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Clusterization.UI
{
    public class AlgorithmSettings : MonoBehaviour
    {
        [SerializeField] protected Button ComputeButton;
        [SerializeField] protected GameObject PanelLocker;
        
        public event Action<MonoBehaviour, IClusterizationAlgorithm> OnComputePressed;

        private void Awake()
        {
            OnComputePressed += (_, __) => enabled = false;
        }

        private void OnEnable()
        {
            ComputeButton.interactable = true;
            PanelLocker.SetActive(false);
        }

        private void OnDisable()
        {
            ComputeButton.interactable = false;
            PanelLocker.SetActive(true);
        }

        protected void InvokeComputePressed(MonoBehaviour sender, IClusterizationAlgorithm algorithm)
        {
            OnComputePressed?.Invoke(sender, algorithm);
        }
    }
}