using Elektronik.Clusterization.Algorithms;
using Elektronik.NativeMath;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Clusterization.UI
{
    public class PlanesDetectionAlgorithmSettings : AlgorithmSettings<PlanesDetectionAlgorithm>
    {
        public TMP_InputField DepthThreshold;
        public TMP_InputField Epsilon;
        public TMP_InputField NumStartPoints;
        public TMP_InputField NumPoints;
        public TMP_InputField Steps;
        public TMP_InputField CountRation;
        public TMP_InputField DCos;
        public TMP_InputField GravityX;
        public TMP_InputField GravityY;
        public TMP_InputField GravityZ;
        public Toggle UseGravity;
        public TMP_InputField GravityDeltaAngle;

        private void Start()
        {
            ComputeButton.OnClickAsObservable()
                    .Subscribe(k => OnComputePressed?.Invoke(this, CreateAlgorithm()));
        }

        private PlanesDetectionAlgorithm CreateAlgorithm()
        {
            return new PlanesDetectionAlgorithm(new Preferences()
            {
                DepthThreshold = int.Parse(DepthThreshold.text),
                Epsilon = double.Parse(Epsilon.text),
                NumStartPoints = int.Parse(NumStartPoints.text),
                NumPoints = int.Parse(NumPoints.text),
                Steps = int.Parse(Steps.text),
                CountRatio = double.Parse(CountRation.text),
                DCos = Mathf.Deg2Rad * float.Parse(DCos.text),
                UseGravity = UseGravity.isOn,
                Gravity = new NativeVector(float.Parse(GravityX.text), float.Parse(GravityY.text), float.Parse(GravityZ.text)),
                GravityDCos = Mathf.Cos(Mathf.Deg2Rad * float.Parse(GravityDeltaAngle.text))
            });
        }
    }
}