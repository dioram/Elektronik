using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.UI.Buttons;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> Turns on/off traces for data updates. </summary>
    [RequireComponent(typeof(ButtonChangingIcons))]
    internal class TraceButton : SourceTreeButton<ITraceableDataSource, ButtonChangingIcons>
    {
        /// <inheritdoc />
        protected override void Initialize(ITraceableDataSource dataSource, ButtonChangingIcons uiButton)
        {
            uiButton.OnStateChangedAsObservable.Subscribe(state => dataSource.TraceEnabled = state == 1).AddTo(this);
        }
    }
}