using Elektronik.Cameras;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.UI.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elektronik.UI
{
    [RequireComponent(typeof(SourceTreeElement))]
    public class SourceTreeButtons : MonoBehaviour
    {
        public Button WindowButton;
        public ButtonChangingIcons VisibleButton;
        public ButtonChangingIcons TraceButton;
        public Button RemoveButton;
        public Button CameraButton;
        public Button SaveButton;

        private ISourceTree _node;

        public void Start()
        {
            _node = GetComponent<SourceTreeElement>().Node;
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (_node is IRendersToWindow rendersToWindow)
            {
                WindowButton.OnClickAsObservable().Subscribe(_ => rendersToWindow.Window.Show());
            }
            else
            {
                WindowButton.gameObject.SetActive(false);
            }

            if (_node is IVisible v)
            {
                VisibleButton.OnStateChanged += state => v.IsVisible = state == 0;
            }
            else
            {
                VisibleButton.gameObject.SetActive(false);
            }

            if (_node is ITraceable t)
            {
                TraceButton.OnStateChanged += state => t.TraceEnabled = state == 1;
            }
            else
            {
                TraceButton.gameObject.SetActive(false);
            }

            if (_node is IRemovable r)
            {
                RemoveButton.OnClickAsObservable().Subscribe(_ =>
                {
                    r.RemoveSelf();
                    Destroy(gameObject);
                });
            }
            else
            {
                RemoveButton.gameObject.SetActive(false);
            }

            if (_node is ILookable lookable && Camera.main.GetComponent<LookableCamera>() is { } cam)
            {
                CameraButton.OnClickAsObservable().Subscribe(_ => cam.Look(lookable.Look(cam.transform)));
            }
            else
            {
                CameraButton.gameObject.SetActive(false);
            }

            if (_node is ISave s)
            {
                SaveButton.OnClickAsObservable().Subscribe(_ => s.Save());
            }
            else
            {
                SaveButton.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!(_node is IVisible v)) return;
            
            VisibleButton.State = v.IsVisible ? 0 : 1;
            VisibleButton.gameObject.SetActive(v.ShowButton);
        }
    }
}