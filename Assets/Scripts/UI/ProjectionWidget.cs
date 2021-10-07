using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    public class ProjectionWidget : MonoBehaviour
    {
        #region Editor fields

        [SerializeField] private Button Button;
        [SerializeField] private Image Icon;
        [SerializeField] private TMP_Text Label;

        [Space]
        [SerializeField] private string PerspectiveText = "Persp";
        [SerializeField] private string OrthographicText = "Ortho";
        [SerializeField] private Sprite PerspectiveSprite;
        [SerializeField] private Sprite OrthographicSprite;

        #endregion

        #region Unity events

        private void Start()
        {
            Button.OnClickAsObservable()
                    .Select(_ => !_isOrthographic)
                    .Subscribe(ChangeProjection)
                    .AddTo(this);
            ChangeProjection(false);
        }

        #endregion

        #region Private

        private bool _isOrthographic;

        private void ChangeProjection(bool isOrthographic)
        {
            _isOrthographic = isOrthographic;
            Camera.main.orthographic = isOrthographic;
            Label.SetLocalizedText(isOrthographic ? OrthographicText : PerspectiveText);
            Icon.sprite = isOrthographic ? OrthographicSprite : PerspectiveSprite;
        }

        #endregion
    }
}