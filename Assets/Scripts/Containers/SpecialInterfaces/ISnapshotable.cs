using System;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using Elektronik.PluginsSystem;
using Newtonsoft.Json.Linq;

namespace Elektronik.Containers.SpecialInterfaces
{
    public interface ISnapshotable
    {
        ISnapshotable TakeSnapshot();

        void WriteSnapshot(IDataRecorderPlugin recorder);
    }

    public static class SnapshotableDeserializer
    {
        public static ISourceTree Deserialize(JToken token) =>
                token["type"].ToString() switch
                {
                    "virtual" => VirtualContainer.Deserialize(token),
                    nameof(SlamPoint) => CloudContainer<SlamPoint>.Deserialize(token),
                    nameof(SlamObservation) => CloudContainer<SlamObservation>.Deserialize(token),
                    nameof(SlamInfinitePlane) => CloudContainer<SlamInfinitePlane>.Deserialize(token),
                    nameof(SlamLine) => SlamLinesContainer.Deserialize(token),
                    nameof(SlamTrackedObject) => TrackedObjectsContainer.Deserialize(token),
                    _ => throw new NotSupportedException($"Token type {token["type"]} is not supported!")
                };
    }
}