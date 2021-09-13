using System;
using System.Linq;
using System.Reflection;
using Elektronik.Containers;
using Elektronik.Data.PackageObjects;
using UnityEngine;

namespace Elektronik.Clouds
{
    public abstract class DataComponent<TCloudItem> : MonoBehaviour where TCloudItem : struct, ICloudItem
    {
        public TCloudItem Data;
        public IContainer<TCloudItem> Container;

        public static Type GetInstantiable()
        {
            if (_instantiable == null)
            {
                _instantiable = Assembly
                        .GetExecutingAssembly()
                        .GetTypes()
                        .FirstOrDefault(t => t.BaseType is { IsGenericType: true } 
                                                && t.BaseType.GetGenericTypeDefinition() == typeof(DataComponent<>) 
                                                && t.BaseType.GetGenericArguments().Contains(typeof(TCloudItem)));
            }

            return _instantiable;
        }

        // ReSharper disable once StaticMemberInGenericType
        private static Type _instantiable;
    }

    public class ObservationData : DataComponent<SlamObservation>
    {
    }

    public class TrackedObjectData : DataComponent<SlamTrackedObject>
    {
    }
}