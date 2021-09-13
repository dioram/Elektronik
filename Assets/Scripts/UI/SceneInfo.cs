using Elektronik.Clouds;
using Elektronik.UI.Localization;
using TMPro;
using UnityEngine;

namespace Elektronik.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class SceneInfo : MonoBehaviour
    {
        [SerializeField] private PointCloudRenderer PointCloudRenderer;
        [SerializeField] private LineCloudRenderer LineCloudRenderer;
        [SerializeField] private InfinitePlaneCloudRenderer InfinitePlaneCloudRenderer;
        [SerializeField] private ObservationCloudRenderer ObservationCloudRenderer;
        [SerializeField] private TrackedObjectCloud TrackedObjectCloud;

        [HideInInspector] public int PointsCount;
        [HideInInspector] public int LinesCount;
        [HideInInspector] public int InfinitePlanesCount;
        [HideInInspector] public int ObservationsCount;
        [HideInInspector] public int TrackedObjectsCount;

        private TMP_Text _label;

        private void Start()
        {
            _label = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            bool isUpdated = UpdateValue(ref PointsCount, PointCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref LinesCount, LineCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref InfinitePlanesCount, InfinitePlaneCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref ObservationsCount, ObservationCloudRenderer.ItemsCount);
            isUpdated = isUpdated || UpdateValue(ref TrackedObjectsCount, TrackedObjectCloud.ItemsCount);

            if (isUpdated)
            {
                _label.SetLocalizedText("Scene info", PointsCount, LinesCount, InfinitePlanesCount, ObservationsCount,
                                        TrackedObjectsCount);
            }
        }

        private bool UpdateValue(ref int value, int newValue)
        {
            if (value == newValue) return false;
            value = newValue;
            return true;
        }
    }
}