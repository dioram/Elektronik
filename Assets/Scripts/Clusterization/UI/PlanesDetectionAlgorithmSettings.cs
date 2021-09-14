using Elektronik.Clusterization.Algorithms;
using Elektronik.Clusterization.Algorithms.PlanesDetectionNative;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.Clusterization.UI
{
    public class PlanesDetectionAlgorithmSettings : MonoBehaviour// : AlgorithmSettings
    {
//         [SerializeField] private TMP_InputField DepthThreshold;
//         [SerializeField] private TMP_InputField Epsilon;
//         [SerializeField] private TMP_InputField NumStartPoints;
//         [SerializeField] private TMP_InputField NumPoints;
//         [SerializeField] private TMP_InputField Steps;
//         [SerializeField] private TMP_InputField CountRation;
//         [SerializeField] private TMP_InputField DCos;
//         [SerializeField] private TMP_InputField GravityX;
//         [SerializeField] private TMP_InputField GravityY;
//         [SerializeField] private TMP_InputField GravityZ;
//         [SerializeField] private Toggle UseGravity;
//         [SerializeField] private TMP_InputField GravityDeltaAngle;
//
// #if !NO_PLANES_DETECTION
//         private void Start()
//         {
//             ComputeButton.OnClickAsObservable()
//                     .Subscribe(k => InvokeComputePressed(this, CreateAlgorithm()));
//         }
//
//         private PlanesDetectionAlgorithm CreateAlgorithm()
//         {
//             return new PlanesDetectionAlgorithm(new Preferences()
//             {
//                 DepthThreshold = int.Parse(DepthThreshold.text),
//                 Epsilon = double.Parse(Epsilon.text),
//                 NumStartPoints = int.Parse(NumStartPoints.text),
//                 NumPoints = int.Parse(NumPoints.text),
//                 Steps = int.Parse(Steps.text),
//                 CountRatio = double.Parse(CountRation.text),
//                 DCos = Mathf.Deg2Rad * float.Parse(DCos.text),
//                 UseGravity = UseGravity.isOn,
//                 Gravity = new Vector3d(float.Parse(GravityX.text), float.Parse(GravityY.text),
//                                        float.Parse(GravityZ.text)),
//                 GravityDCos = Mathf.Cos(Mathf.Deg2Rad * float.Parse(GravityDeltaAngle.text))
//             });
//         }
// #else
//         private void Start()
//         {
//             gameObject.SetActive(false);
//         }
// #endif
    }
}