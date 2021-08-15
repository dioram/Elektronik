using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elektronik.Containers;
using Elektronik.Containers.EventArgs;
using Elektronik.Containers.SpecialInterfaces;
using Elektronik.Data;
using Elektronik.Data.PackageObjects;
using UniRx;

namespace Elektronik.PluginsSystem
{
    public static class DataRecorderSubscriber
    {
        public static readonly Dictionary<IDataRecorderPlugin, Dictionary<ISourceTree, List<IDisposable>>>
                Subscriptions =
                        new Dictionary<IDataRecorderPlugin, Dictionary<ISourceTree, List<IDisposable>>>();

        public static bool SubscribeOn(this IDataRecorderPlugin recorder, ISourceTree source, string topicName)
        {
            if (!Subscriptions.ContainsKey(recorder))
                Subscriptions.Add(recorder, new Dictionary<ISourceTree, List<IDisposable>>());
            if (Subscriptions[recorder].ContainsKey(source)) return true;
            Subscriptions[recorder].Add(source, new List<IDisposable>());

            if (source is IRemovable r)
            {
                r.OnRemoved += () => recorder.UnsubscribeFrom(source);
            }

            var type = source.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType &&
                                            x.GetGenericTypeDefinition() == typeof(IConnectableObjectsContainer<>));
            if (type != null)
            {
                typeof(DataRecorderSubscriber)
                        .GetMethod(nameof(SubscribeOnConnectableContainer),
                                   BindingFlags.Static | BindingFlags.NonPublic)?
                        .MakeGenericMethod(type.GetGenericArguments()[0])
                        .Invoke(null, new object[] {recorder, source, topicName});
                return true;
            }

            type = source.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IContainer<>));
            if (type != null)
            {
                typeof(DataRecorderSubscriber)
                        .GetMethod(nameof(SubscribeOnCloudContainer), BindingFlags.Static | BindingFlags.NonPublic)?
                        .MakeGenericMethod(type.GetGenericArguments()[0])
                        .Invoke(null, new object[] {recorder, source, topicName});
                return true;
            }

            return false;
        }
        
        private static void SubscribeOnConnectableContainer<TCloudItem>(this IDataRecorderPlugin recorder,
                                                                        ISourceTree source,
                                                                        string topicName)
                where TCloudItem : struct, ICloudItem
        {
            var connectionsUpdatedName = nameof(IConnectableObjectsContainer<ICloudItem>.OnConnectionsUpdated);
            var connectionsUpdated = Observable.FromEvent<EventHandler<ConnectionsEventArgs>>(
                        h => (sender, args) => recorder.OnConnectionsUpdated<TCloudItem>(topicName, args.Items.ToList()),
                        h => source.GetType().GetEvent(connectionsUpdatedName).AddEventHandler(source, h),
                        h => source.GetType().GetEvent(connectionsUpdatedName).RemoveEventHandler(source, h))
                    .Subscribe();

            var connectionsRemovedName = nameof(IConnectableObjectsContainer<ICloudItem>.OnConnectionsRemoved);
            var connectionsRemoved = Observable.FromEvent<EventHandler<ConnectionsEventArgs>>(
                        h => (sender, args) => recorder.OnConnectionsRemoved<TCloudItem>(topicName, args.Items.ToList()),
                        h => source.GetType().GetEvent(connectionsRemovedName).AddEventHandler(source, h),
                        h => source.GetType().GetEvent(connectionsRemovedName).RemoveEventHandler(source, h))
                    .Subscribe();

            Subscriptions[recorder][source].Add(connectionsUpdated);
            Subscriptions[recorder][source].Add(connectionsRemoved);
            recorder.OnConnectionsUpdated<TCloudItem>(source.DisplayName, (source as IConnectableObjectsContainer<TCloudItem>)?
                                                      .Connections
                                                      .Select(l => (l.Point1.Id, l.Point2.Id))
                                                      .ToList());
            recorder.SubscribeOnCloudContainer<TCloudItem>(source, topicName);
        }

        private static void SubscribeOnCloudContainer<TCloudItem>(this IDataRecorderPlugin recorder, ISourceTree source,
                                                                  string topicName)
                where TCloudItem : struct, ICloudItem
        {
            var addedName = nameof(IContainer<ICloudItem>.OnAdded);
            var add = Observable.FromEvent<EventHandler<AddedEventArgs<TCloudItem>>>(
                        h => (sender, args) => recorder.OnAdded(topicName, args.AddedItems.ToList()),
                        h => source.GetType().GetEvent(addedName).AddEventHandler(source, h),
                        h => source.GetType().GetEvent(addedName).RemoveEventHandler(source, h))
                    .Subscribe();

            var updatedName = nameof(IContainer<ICloudItem>.OnUpdated);
            var updated = Observable.FromEvent<EventHandler<UpdatedEventArgs<TCloudItem>>>(
                        h => (sender, args) => recorder.OnUpdated(topicName, args.UpdatedItems.ToList()),
                        h => source.GetType().GetEvent(updatedName).AddEventHandler(source, h),
                        h => source.GetType().GetEvent(updatedName).RemoveEventHandler(source, h))
                    .Subscribe();

            var removedName = nameof(IContainer<ICloudItem>.OnRemoved);
            var removed = Observable.FromEvent<EventHandler<RemovedEventArgs>>(
                        h => (sender, args) => recorder.OnRemoved<TCloudItem>(topicName, args.RemovedIds.ToList()),
                        h => source.GetType().GetEvent(removedName).AddEventHandler(source, h),
                        h => source.GetType().GetEvent(removedName).RemoveEventHandler(source, h))
                    .Subscribe();

            Subscriptions[recorder][source].Add(add);
            Subscriptions[recorder][source].Add(updated);
            Subscriptions[recorder][source].Add(removed);

            recorder.OnAdded(source.DisplayName, (source as IContainer<TCloudItem>)?.ToList());
        }

        public static void UnsubscribeFromEverything(this IDataRecorderPlugin recorder)
        {
            if (!Subscriptions.ContainsKey(recorder)) return;
            foreach (var s in Subscriptions[recorder])
            {
                foreach (var disposable in s.Value)
                {
                    disposable.Dispose();
                }
            }

            Subscriptions.Remove(recorder);
        }

        public static void UnsubscribeFrom(this IDataRecorderPlugin recorder, ISourceTree source)
        {
        }
    }
}