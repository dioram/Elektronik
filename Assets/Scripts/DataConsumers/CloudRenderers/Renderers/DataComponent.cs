using System;
using System.Linq;
using System.Reflection;
using Elektronik.DataObjects;
using Elektronik.DataSources.Containers;
using UnityEngine;

namespace Elektronik.DataConsumers.CloudRenderers
{
    /// <summary> Component that contains cloud item info for game object. </summary>
    /// <remarks>
    /// Since Unity can't instantiate generic types this class was made abstract.
    /// Use one of derived classes or <c>DataComponent&lt;TCloudItem&gt;.GetInstantiable()</c> to get type that
    /// can be instantiated in Unity.
    /// </remarks>
    /// <typeparam name="TCloudItem"> Type of cloud item. </typeparam>
    internal abstract class DataComponent<TCloudItem> : MonoBehaviour where TCloudItem : struct, ICloudItem
    {
        public TCloudItem Data;
        public ICloudContainer<TCloudItem> CloudContainer;

        public static Type GetInstantiable()
        {
            if (_instantiable != null) return _instantiable;

            _instantiable = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.BaseType is { IsGenericType: true }
                                            && t.BaseType.GetGenericTypeDefinition() == typeof(DataComponent<>)
                                            && t.BaseType.GetGenericArguments().Contains(typeof(TCloudItem)));
            return _instantiable;
        }

        // ReSharper disable once StaticMemberInGenericType
        private static Type _instantiable;
    }

    /// <summary> Implementation of <see cref="DataComponent{TCloudItem}"/> for <see cref="SlamObservation"/> </summary>
    internal class ObservationData : DataComponent<SlamObservation>
    {
    }

    /// <summary>
    /// Implementation of <see cref="DataComponent{TCloudItem}"/> for <see cref="SlamTrackedObject"/>
    /// </summary>
    internal class TrackedObjectData : DataComponent<SlamTrackedObject>
    {
    }

    /// <summary> Implementation of <see cref="DataComponent{TCloudItem}"/> for <see cref="SlamMarker"/> </summary>
    internal class MarkerObjectData : DataComponent<SlamMarker>
    {
    }
}