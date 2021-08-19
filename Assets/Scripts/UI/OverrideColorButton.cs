using Elektronik.Mesh;
using Elektronik.UI.Localization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(Button))]
    public class OverrideColorButton: MonoBehaviour
    {
        [SerializeField] private MeshContainerGpuRenderer Renderer;
        
        private string GetText(bool overrided)
        {
            return overrided ? "Use vertex colors for mesh" : "Override colors for mesh"; 
        }
        
        private void Start()
        {
            var button = GetComponent<Button>();
            var text = button.GetComponentInChildren<TMP_Text>();
            text.SetLocalizedText(GetText(false));
            button.OnClickAsObservable()
                    .Select(_ => !Renderer.OverrideColors)
                    .Do(v => Renderer.OverrideColors = v)
                    .Do(v => text.SetLocalizedText(GetText(v)))
                    .Subscribe();
        }
    }
}