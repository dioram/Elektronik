namespace VRTK.UnityEventHelper
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_ObjectAutoGrab_UnityEvents")]
    public sealed class VRTK_ObjectAutoGrab_UnityEvents : VRTK_UnityEvents<VRTK_ObjectAutoGrab>
    {
        [Serializable]
        public sealed class ObjectAutoGrabEvent : UnityEvent<object> { }

        public ObjectAutoGrabEvent OnObjectAutoGrabCompleted = new ObjectAutoGrabEvent();

        protected override void AddListeners(VRTK_ObjectAutoGrab component)
        {
            component.ObjectAutoGrabCompleted += ObjectAutoGrabCompleted;
        }

        protected override void RemoveListeners(VRTK_ObjectAutoGrab component)
        {
            component.ObjectAutoGrabCompleted -= ObjectAutoGrabCompleted;
        }

        private void ObjectAutoGrabCompleted(object o)
        {
            OnObjectAutoGrabCompleted.Invoke(o);
        }
    }
}