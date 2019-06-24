namespace VRTK.UnityEventHelper
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("VRTK/Scripts/Utilities/Unity Events/VRTK_PositionRewind_UnityEvents")]
    public sealed class VRTK_PositionRewind_UnityEvents : VRTK_UnityEvents<VRTK_PositionRewind>
    {
        [Serializable]
        public sealed class PositionRewindEvent : UnityEvent<object, PositionRewindEventArgs> { }

        public PositionRewindEvent OnPositionRewindToSafe = new PositionRewindEvent();

        protected override void AddListeners(VRTK_PositionRewind component)
        {
            component.PositionRewindToSafe += PositionRewindToSafe;
        }

        protected override void RemoveListeners(VRTK_PositionRewind component)
        {
            component.PositionRewindToSafe -= PositionRewindToSafe;
        }

        private void PositionRewindToSafe(object o, PositionRewindEventArgs e)
        {
            OnPositionRewindToSafe.Invoke(o, e);
        }
    }
}