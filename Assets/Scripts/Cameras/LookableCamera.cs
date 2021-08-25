using System.Collections;
using UnityEngine;

namespace Elektronik.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class LookableCamera : MonoBehaviour
    {
        public float MovingTme = 1f;
        private bool _isMoving;
        public float Scale = 1;
        
        public void Look((Vector3 pos, Quaternion rotation) pose)
        {
            if (_isMoving || pose.pos == transform.position && pose.rotation == transform.rotation) return;
            StartCoroutine(MoveTo(pose.pos * Scale, pose.rotation));
        }

        private IEnumerator MoveTo(Vector3 pos, Quaternion rotation)
        {
            _isMoving = true;
            var startPos = transform.position;
            var startRot = transform.rotation;

            for (int i = 0; i < 100; i++)
            {
                transform.position = Vector3.Lerp(startPos, pos, i / 100f);
                transform.rotation = Quaternion.Lerp(startRot, rotation, i / 100f);
                yield return new WaitForSeconds(MovingTme / 100);
            }

            _isMoving = false;
        }
    }
}