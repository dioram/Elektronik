using Elektronik.DataConsumers.CloudRenderers;
using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;

namespace Elektronik.UI
{
    /// <summary> Component for rendering data about data sources. </summary>
    [RequireComponent(typeof(TMP_Text))]
    internal class SceneInfo : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private PointCloudRendererComponent PointCloudRenderer;
        [SerializeField] private SlamLineCloudRendererComponent SlamLineCloudRenderer;
        [SerializeField] private SimpleLineCloudRendererComponent SimpleLineCloudRenderer;
        [SerializeField] private PlaneCloudRendererComponent PlaneCloudRenderer;
        [SerializeField] private ObservationCloudRendererComponent ObservationCloudRenderer;
        [SerializeField] private TrackedObjectCloud TrackedObjectCloud;

        #endregion

        #region Unity events
        
        private void Start()
        {
            _label = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            // TODO: When (if) using UniTask rewrite it to use events instead update.
            var isUpdated = UpdateValue(ref _pointsCount, PointCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref _slamLinesCount, SlamLineCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref _simpleLinesCount, SimpleLineCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref _infinitePlanesCount, PlaneCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref _observationsCount, ObservationCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref _trackedObjectsCount, TrackedObjectCloud.ItemsCount);

            if (isUpdated)
            {
                _label.SetLocalizedText("Scene info", _pointsCount, _slamLinesCount + _simpleLinesCount,
                                        _infinitePlanesCount, _observationsCount, _trackedObjectsCount);
            }
        }

        #endregion

        #region Private

        private TMP_Text _label;
        private int _pointsCount;
        private int _slamLinesCount;
        private int _simpleLinesCount;
        private int _infinitePlanesCount;
        private int _observationsCount;
        private int _trackedObjectsCount;

        private bool UpdateValue(ref int value, int newValue)
        {
            if (value == newValue) return false;
            value = newValue;
            return true;
        }
        
        #endregion
    }
}