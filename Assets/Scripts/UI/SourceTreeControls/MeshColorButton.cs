using Elektronik.DataSources.Containers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI.SourceTreeControls
{
    /// <summary> When pressed changes rendering mode of mesh. </summary>
    [RequireComponent(typeof(Button))]
    internal class MeshColorButton : SourceTreeButton<IMeshContainer, Button>
    {
        /// <inheritdoc />
        protected override void Initialize(IMeshContainer dataSource, Button uiButton)
        {
            uiButton.OnClickAsObservable().Subscribe(_ => dataSource.SwitchShader()).AddTo(this);
        }
    }
}