using Elektronik.DataSources.SpecialInterfaces;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> When pressed removed data source. </summary>
    [RequireComponent(typeof(Button))]
    internal class RemoveButton : SourceTreeButton<IRemovableDataSource, Button>
    {
        /// <inheritdoc />
        protected override void Initialize(IRemovableDataSource dataSource, Button uiButton)
        {
            uiButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        dataSource.RemoveSelf();
                        Destroy(gameObject);
                    })
                    .AddTo(this);
        }
    }
}