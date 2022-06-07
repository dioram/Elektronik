using Elektronik.DataSources.SpecialInterfaces;
using Elektronik.PluginsSystem.UnitySide;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> WHen pressed saves data source on disk. </summary>
    [RequireComponent(typeof(Button))]
    internal class SaveButton : SourceTreeButton<ISavableDataSource, Button>
    {
        protected override void Initialize(ISavableDataSource dataSource, Button uiButton)
        {
            uiButton.OnClickAsObservable().Subscribe(_ => RecorderPluginsController.Save(dataSource)).AddTo(this);
        }
    }
}