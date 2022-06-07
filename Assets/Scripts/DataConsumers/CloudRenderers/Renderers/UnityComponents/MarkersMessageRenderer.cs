using System;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using Elektronik.UI;
using TMPro;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Implementation of renderer for markers messages. </summary>
    internal class MarkersMessageRenderer : GameObjectCloud<SlamMarker>
    {
        #region Editor fields

        /// <summary> Distance between center of marker and test position. </summary>
        [SerializeField] [Tooltip("Distance between center of marker and test position.")] 
        private float Padding = 0.5f;
        
        /// <summary> Multiplier for font size - scene scale relation. </summary>
        [SerializeField] [Tooltip("Multiplier for font size - scene scale relation.")] 
        private float FontSizeMultiplier = 4;

        #endregion

        /// <inheritdoc />
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

        #region Protected

        /// <inheritdoc />
        protected override Pose GetObjectPose(SlamMarker obj)
        {
            return new Pose(obj.Position, obj.Rotation);
        }

        /// <inheritdoc />
        protected override GameObject AddInMainThread(object sender, SlamMarker item)
        {
            var pose = GetObjectPose(item);
            var go = ObjectsPool.Spawn(pose.position * Scale, pose.rotation);
            GameObjects[(sender.GetHashCode(), item.Id)] = go;

            var dc = go.GetComponent(DataComponent<SlamMarker>.GetInstantiable());
            if (dc == null) dc = go.AddComponent(DataComponent<SlamMarker>.GetInstantiable());
            var dataComponent = (DataComponent<SlamMarker>)dc;
            dataComponent.Data = item;
            dataComponent.CloudContainer = sender as ICloudContainer<SlamMarker>;
            UpdateMessage(go, item);
            return go;
        }

        /// <inheritdoc />
        protected override GameObject UpdateInMainThread(object sender, SlamMarker item)
        {
            var go = GameObjects[(sender.GetHashCode(), item.Id)];
            go.transform.SetPositionAndRotation(item.Position * Scale, item.Rotation);
            go.GetComponent<DataComponent<SlamMarker>>().Data = item;
            UpdateMessage(go, item);
            return go;
        }

        #endregion

        #region Private

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

        #endregion
    }
}