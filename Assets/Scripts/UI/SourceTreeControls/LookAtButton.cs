using Elektronik.Cameras;
using Elektronik.DataSources.SpecialInterfaces;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> When pressed moves camera to see all data from source. </summary>
    [RequireComponent(typeof(Button))]
    internal class LookAtButton : SourceTreeButton<ILookableDataSource, Button>
    {
        /// <inheritdoc />
        protected override void Initialize(ILookableDataSource dataSource, Button uiButton)
        {
            if (Camera.main is null)
            {
                Destroy(gameObject);
                return;
            }

            var cam = Camera.main.GetComponent<LookableCamera>();
            if (cam is null)
            {
                Destroy(gameObject);
                return;
            }
            
            uiButton.OnClickAsObservable().Subscribe(_ => cam.Look(dataSource.Look(cam.transform))).AddTo(this);
        }
    }
}