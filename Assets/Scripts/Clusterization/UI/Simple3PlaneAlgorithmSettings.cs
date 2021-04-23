using Elektronik.Clusterization.Algorithms;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.Clusterization.UI
{
    public class Simple3PlaneAlgorithmSettings : AlgorithmSettings<Simple3PlaneClusterization>
    {
        public TMP_InputField XInput;
        public TMP_InputField YInput;
        public TMP_InputField ZInput;

        private void Start()
        {
            ComputeButton.OnClickAsObservable()
                    .Select(_ => new Vector3(float.Parse(XInput.text),
                                             float.Parse(YInput.text),
                                             float.Parse(ZInput.text)))
                    .Subscribe(v => OnComputePressed?.Invoke(this, new Simple3PlaneClusterization(v)));
        }
    }
}