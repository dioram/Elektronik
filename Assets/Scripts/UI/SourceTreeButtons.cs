using Elektronik.Cameras;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.UI.Windows;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("CameraButton")] public Button LookAtButton;
        public ButtonChangingIcons FollowButton;
        public Button SaveButton;

        public Button WeightButton;
        public Slider WeightSlider;
        public TMP_Text MinWeightLabel;
        public TMP_Text MaxWeightLabel;

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

            if (Camera.main.GetComponent<LookableCamera>() is { } cam)
            {
                switch (_node)
                {
                case IFollowable<SlamTrackedObject> followable when _node is ILookable l:
                    FollowButton.OnStateChanged += state =>
                    {
                        if (state == 0)
                        {
                            followable.Unfollow();
                        }
                        else
                        {
                            cam.Look(l.Look(cam.transform));
                            followable.Follow();
                        }
                    };
                    LookAtButton.gameObject.SetActive(false);
                    break;
                case ILookable lookable:
                    LookAtButton.OnClickAsObservable().Subscribe(_ => cam.Look(lookable.Look(cam.transform)));
                    FollowButton.gameObject.SetActive(false);
                    break;
                default:
                    LookAtButton.gameObject.SetActive(false);
                    FollowButton.gameObject.SetActive(false);
                    break;
                }
            }

            if (_node is ISave s)
            {
                SaveButton.OnClickAsObservable().Subscribe(_ => s.Save());
            }
            else
            {
                SaveButton.gameObject.SetActive(false);
            }

            if (_node is IWeightable w)
            {
                void SetMaxWeight(int value)
                {
                    MaxWeightLabel.text = $"{value}";
                    WeightSlider.maxValue = value;
                }
                w.OnMaxWeightChanged += SetMaxWeight;
                SetMaxWeight(w.MaxWeight);
                WeightSlider.OnValueChangedAsObservable()
                        .Do(value => MinWeightLabel.text = $"{(int) value}")
                        .Subscribe(value => w.MinWeight = (int) value);
            }
            else
            {
                WeightButton.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (_node is IVisible v)
            {
                VisibleButton.State = v.IsVisible ? 0 : 1;
                VisibleButton.gameObject.SetActive(v.ShowButton);
            }

            if (_node is IFollowable<SlamTrackedObject> f)
            {
                FollowButton.SilentSetState(f.IsFollowed ? 1 : 0);
            }
        }
    }
}