using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.UI.Buttons;
using UniRx;
using UnityEngine;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> Turns on/off visibility of data source. </summary>
    [RequireComponent(typeof(ButtonChangingIcons))]
    internal class VisibilityButton : SourceTreeButton<IVisibleDataSource, ButtonChangingIcons>
    {
        /// <inheritdoc />
        protected override void Initialize(IVisibleDataSource dataSource, ButtonChangingIcons uiButton)
        {
            uiButton.OnStateChangedAsObservable.Subscribe(state => dataSource.IsVisible = state == 0).AddTo(this);
            uiButton.State = dataSource.IsVisible ? 0 : 1;
        }
        
        private void Update()
        {
            UiButton.SilentSetState(DataSource.IsVisible ? 0 : 1);
        }
    }
}