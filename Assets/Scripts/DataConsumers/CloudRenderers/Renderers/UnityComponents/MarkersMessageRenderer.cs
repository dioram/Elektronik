using System;
using Elektronik.Data.PackageObjects;
using Elektronik.DataSources.Containers;
using Elektronik.DataSources.Containers.EventArgs;
using Elektronik.UI;
using TMPro;
using UniRx;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    public class MarkersMessageRenderer : GameObjectCloud<SlamMarker>
    {
        public float Padding = 0.5f;
        public float FontSizeMultiplier = 4;

        public override void OnItemsAdded(object sender, AddedEventArgs<SlamMarker> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.AddedItems)
                {
                    var pose = GetObjectPose(obj);
                    UniRxExtensions.StartOnMainThread(() =>
                    {
                        var go = ObjectsPool.Spawn(pose.position * Scale, pose.rotation);
                        GameObjects[(sender.GetHashCode(), obj.Id)] = go;

                        var dc = go.GetComponent(DataComponent<SlamMarker>.GetInstantiable());
                        if (dc == null) dc = go.AddComponent(DataComponent<SlamMarker>.GetInstantiable());
                        var dataComponent = (DataComponent<SlamMarker>)dc;
                        dataComponent.Data = obj;
                        dataComponent.Container = sender as IContainer<SlamMarker>;
                        UpdateMessage(go, obj);
                    }).Subscribe();
                }
            }
        }

        public override void OnItemsUpdated(object sender, UpdatedEventArgs<SlamMarker> e)
        {
            if (!IsSenderVisible(sender)) return;
            lock (GameObjects)
            {
                foreach (var obj in e.UpdatedItems)
                {
                    var pose = GetObjectPose(obj);
                    UniRxExtensions.StartOnMainThread(() =>
                    {
                        var go = GameObjects[(sender.GetHashCode(), obj.Id)];
                        go.transform.SetPositionAndRotation(pose.position * Scale, pose.rotation);
                        go.GetComponent<DataComponent<SlamMarker>>().Data = obj;
                        UpdateMessage(go, obj);
                    }).Subscribe();
                }
            }
        }

        public override float Scale
        {
            get => _scale;
            set
            {
                if (Math.Abs(_scale - value) < float.Epsilon) return;

                lock (GameObjects)
                {
                    var factor = value / _scale;
                    _scale = value;
                    foreach (var go in GameObjects.Values)
                    {
                        go.transform.position *= factor;
                        var obj = go.GetComponent<MarkerObjectData>().Data;
                        go.transform.position = obj.Position * _scale;
                        go.GetComponentInChildren<TMP_Text>().fontSize = _scale * FontSizeMultiplier;
                        go.GetComponentInChildren<LookAtCamera>().Radius = (obj.Scale.y * 0.7f + Padding) * _scale;
                    }
                }
            }
        }

        protected override Pose GetObjectPose(SlamMarker obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        private void UpdateMessage(GameObject go, SlamMarker obj)
        {
            go.transform.position = obj.Position * Scale;
            go.GetComponentInChildren<LookAtCamera>().Radius = (obj.Scale.y * 0.5f + Padding) * _scale;
            var label = go.GetComponentInChildren<TMP_Text>();
            label.text = $"{obj.Message}";
            label.color = obj.Color;
            label.fontSize = _scale * FontSizeMultiplier;
            go.SetActive(!string.IsNullOrEmpty(obj.Message) && gameObject.activeInHierarchy);
        }

        private float _scale = 1;
    }
}