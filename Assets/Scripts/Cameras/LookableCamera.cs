using System.Collections;
using UnityEngine;

namespace Elektronik.Cameras
{
    /// <summary> This component allows other objects to call Look at animation for camera. </summary>
    [RequireComponent(typeof(Camera))]
    public class LookableCamera : MonoBehaviour
    {
        #region Editor fields

        /// <summary> Time interval for camera Look at animation. </summary>
        [SerializeField] [Tooltip("Time interval for camera Look at animation.")]
        private float MovingTme = 1f;

        #endregion
        
        /// <summary> Scale of scene. </summary>
        [Tooltip("Scale of scene.")] public float SceneScale = 1;

        public void Look(Pose pose)
        {
            if (_isMoving || pose.position == transform.position && pose.rotation == transform.rotation) return;
            StartCoroutine(MoveTo(pose.position * SceneScale, pose.rotation));
        }

        #region Private
        
        private bool _isMoving;

        private IEnumerator MoveTo(Vector3 pos, Quaternion rotation)
        {
            _isMoving = true;
            var startPos = transform.position;
            var startRot = transform.rotation;

            for (var i = 0; i < 100; i++)
            {
                transform.position = Vector3.Lerp(startPos, pos, i / 100f);
                transform.rotation = Quaternion.Lerp(startRot, rotation, i / 100f);
                yield return new WaitForSeconds(MovingTme / 100);
            }

            _isMoving = false;
        }

        #endregion
    }
}