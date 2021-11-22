using Elektronik.DataSources.SpecialInterfaces;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> When pressed shows window for this data source. </summary>
    [RequireComponent(typeof(Button))]
    internal class ShowWindowButton : SourceTreeButton<IRendersToWindow, Button>
    {
        /// <inheritdoc />
        protected override void Initialize(IRendersToWindow dataSource, Button uiButton)
        {
            uiButton.OnClickAsObservable().Subscribe(_ => dataSource.Window.Show()).AddTo(this);
        }
    }
}