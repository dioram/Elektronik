using Elektronik.Clusterization.Algorithms;
using TMPro;
using UniRx;

namespace Elektronik.Clusterization.UI
{
    public class KMeansAlgorithmSettings : AlgorithmSettings
    {
        public TMP_InputField KInput;

        private void Start()
        {
            ComputeButton.OnClickAsObservable()
                    .Select(_ => int.Parse(KInput.text))
                    .Subscribe(k => InvokeComputePressed(this, new KMeansClusterization(k)));
        }
    }
}