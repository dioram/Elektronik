using UnityEngine;

namespace Elektronik.Common.Events
{
    public class RuntimeGameObjectAttacher : MonoBehaviour
    {
        public Transform attachTo;

        void Start()
        {
            if (attachTo != null)
            {
                transform.SetParent(attachTo);
            }
        }
    }
}