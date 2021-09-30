﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Data.PackageObjects;
using Elektronik.DataConsumers.Collision;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.Input;
using Elektronik.Threading;
using Elektronik.UI.Windows;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elektronik.UI
{
    public class ObservationToolTip : MonoBehaviour
    {
        [SerializeField] private ObservationCollisionCloud CollisionCloud;
        [SerializeField] private WindowsManager Manager;
        [SerializeField] [Range(0, 1f)] private float CollisionCheckTimeout = 0.1f;

        #region Unity events

        void Start()
        {
            _camera = Camera.main;
            Manager.CreateWindow<ObservationViewer>($"", (viewer, window) =>
            {
                _floatingViewer = viewer;
                viewer.Hide();
            });
            StartCoroutine(CheckCollisionCoroutine());

            var controls = new CameraControls().Default;
            controls.Enable();
            controls.Click.PerformedAsObservable()
                    .Select(_ => Mouse.current.position.ReadValue())
                    .Select(v => _camera.ScreenPointToRay(v))
                    .Select(ray => CollisionCloud.FindCollided(ray))
                    .Where(data => data.HasValue)
                    // ReSharper disable once PossibleInvalidOperationException
                    .Select(v => v.Value)
                    .ObserveOnMainThread()
                    .Do(data => CreateOrShowWindow(data.container, data.item, "Observation #{0}"))
                    .Subscribe()
                    .AddTo(this);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Private

        private Camera _camera;
        private ObservationViewer _floatingViewer;
        private readonly List<ObservationViewer> _pinnedViewers = new List<ObservationViewer>();

        private IEnumerator CheckCollisionCoroutine()
        {
            while (true)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                var ray = _camera.ScreenPointToRay(mousePosition);
                Task.Run(() =>
                {
                    var collision = CollisionCloud.FindCollided(ray);

                    if (collision.HasValue)
                    {
                        MainThreadInvoker.Enqueue(
                            () => ProcessRaycast(collision.Value.container, collision.Value.item, mousePosition));
                    }
                    else
                    {
                        MainThreadInvoker.Enqueue(HideViewer);
                    }
                });
                yield return new WaitForSeconds(CollisionCheckTimeout);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void ProcessRaycast(IContainer<SlamObservation> container, SlamObservation observation,
                                    Vector3 mousePosition)
        {
            _floatingViewer.Render((container, observation));
            _floatingViewer.transform.position = mousePosition + new Vector3(15, -15);
        }

        private void HideViewer()
        {
            if (_floatingViewer != null) _floatingViewer.Hide();
        }

        private void CreateOrShowWindow(IContainer<SlamObservation> container, SlamObservation observation,
                                        string title)
        {
            var v = _pinnedViewers.FirstOrDefault(w => w.ObservationContainer == container.GetHashCode()
                                                          && w.ObservationId == observation.Id);

            if (v is null)
            {
                container.OnRemoved += DestroyObsoleteWindows;
                Manager.CreateWindow<ObservationViewer>(title, (viewer, window) =>
                                                        {
                                                            viewer.Render((container, observation));
                                                            _pinnedViewers.Add(
                                                                window.GetComponent<ObservationViewer>());
                                                        },
                                                        observation.Id);
            }
            else
            {
                v.GetComponent<Window>().Show();
            }
        }

        private void DestroyObsoleteWindows(object container, RemovedEventArgs<SlamObservation> args)
        {
            foreach (var obs in args.RemovedItems)
            {
                var v = _pinnedViewers.FirstOrDefault(w => w.ObservationContainer == container.GetHashCode()
                                                              && w.ObservationId == obs.Id);
                if (v != null)
                {
                    MainThreadInvoker.Enqueue(() =>
                    {
                        Destroy(v.gameObject);
                        _pinnedViewers.Remove(v);
                    });
                }
            }
        }

        #endregion
    }
}