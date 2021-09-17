### Elektronik's architecture is based on MVC patter.

- Model is classes implementing [ISourceTreeNode](../Assets/Scripts/DataSources/ISourceTreeNode.cs) interface. This classes we calling Data Sources.
- View is classes implementing [ISourceRenderer](../Assets/Scripts/DataConsumers/IDataConsumer.cs). This classes we calling Data Consumers.
- Class [DataSourcesController](../Assets/Scripts/DataControllers/DataSourcesController.cs) subscribes renderers to data sources.

![diagram](Images/ElektronikDiagram.png)

## Model

Data sources can be divided on three groups:
- Cloud data containers. They contains data that can be rendered on unity scene such as points, lines, planes, meshes, observations, tracked objects.
  This containers implements [IContainer\<T\>](../Assets/Scripts/DataSources/Containers/IContainer.cs) interface.
  Reference implementation: [CloudContainerBase\<T\>](../Assets/Scripts/DataSources/Containers/CloudContainerBase.cs).
- Virtual. They don't contains any information and just help to organize other sources into tree structure. Implemented in [VirtualSource](../Assets/Scripts/DataSources/Containers/VirtualSource.cs).
- All others. They contains data that can't be rendered on scene such as text, images, etc.
  This sources implements [IRendersToWindow](../Assets/Scripts/DataSources/SpecialInterfaces/IRendersToWindow.cs) interface.

Data sources can implement other special interface that allows user ot interact with them through data tree window.
- ![button](Images/VisibilityButton.png) [IVisible](../Assets/Scripts/DataSources/SpecialInterfaces/IVisible.cs) Toggles source visibility.
  *On disabling visibility sends signal to renderer about removing all points.* Reference implementation: [CloudContainer\<T\>](../Assets/Scripts/DataSources/Containers/CloudContainer.cs).
- ![button](Images/LookAtButton.png) [ILookable](../Assets/Scripts/DataSources/SpecialInterfaces/ILookable.cs) Allows to move camera to see all data from container.
  *For given camera coordinates returns nearest coordinates from where camera will see all objects from this container* Reference implementation: [CloudContainer\<T\>](../Assets/Scripts/DataSources/Containers/CloudContainer.cs).
- ![button](Images/LookAtButton.png) [IFollowable](../Assets/Scripts/DataSources/SpecialInterfaces/IFollowable.cs) Allows to make camera follow object.
  Reference implementation: [TrackContainer](../Assets/Scripts/DataSources/Containers/TrackContainer.cs).
- ![button](Images/DeleteButton.png) [IRemovable](../Assets/Scripts/DataSources/SpecialInterfaces/IRemovable.cs) Allows to remove this data source from tree.
  Reference implementation: [CloudContainer\<T\>](../Assets/Scripts/DataSources/Containers/ClustersContainer.cs).
- ![button](Images/SaveButton.png) [ISave](../Assets/Scripts/DataSources/SpecialInterfaces/ISave.cs) Marks that this data source can be saved to file.
- ![button](Images/TraceButton.png) [ITraceable](../Assets/Scripts/DataSources/SpecialInterfaces/ITraceable.cs) Marks that objects from this container can leave a trace.
  Reference implementation: [CloudContainer\<T\>](../Assets/Scripts/DataSources/Containers/CloudContainer.cs).
- ![button](Images/Connections.png) [IWeightable](../Assets/Scripts/DataSources/SpecialInterfaces/IWeightable.cs)  Reference implementation: [Connector](../Assets/Scripts/DataSources/Containers/Connector.cs).
- ![button](Images/OpenWindowButton.png) [IRendersToWindow](../Assets/Scripts/DataSources/SpecialInterfaces/IRendersToWindow.cs) Marks that content of this source mast be rendered not on scene but in window.
- [IClusterable](../Assets/Scripts/DataSources/SpecialInterfaces/IClusterable.cs) Marks that this data source contains point cloud that can be divided into clusters.

Model's structure and adding/updating/removing data from/to it is controlled by [plugins](Plugins-EN.md).

## View

Consumers can be divided on two groups:
- For rendering cloud data (points, lines, planes, meshes, observations, tracked objects) on scene. 
  They implement [ICloudRenderer\<T\>](../Assets/Scripts/DataConsumers/CloudRenderers/Renderers/ICloudRenderer.cs) or [IMeshRenderer](../Assets/Scripts/DataConsumers/CloudRenderers/Renderers/IMeshRenderer.cs) interfaces.
  Reference implementation: [CloudRenderer\<T\>](../Assets/Scripts/DataConsumers/CloudRenderers/Renderers/CloudRenderer.cs).
- For rendering all other data (text, images, tables) in separate windows.
  They implements [IDataRenderer\<T\>](../Assets/Scripts/DataConsumers/Windows/IDataRenderer.cs) interface.
  Reference implementation: [ImageRenderer](../Assets/Scripts/DataConsumers/Windows/ImageRenderer.cs).

## Controller

When new source emerged controller sends all registered renderers to it via `ISourceTree.AddRenderer(ISourceRenderer)` method.
Source should check if this renderer is supported and if so then subscribe it to events. After that propagate renderer to children.
Example:
```c#
if (renderer is ICloudRenderer<TCloudItem> typedRenderer)
{
    OnAdded += typedRenderer.OnItemsAdded;
    OnUpdated += typedRenderer.OnItemsUpdated;
    OnRemoved += typedRenderer.OnItemsRemoved;
    if (Count > 0)
    {
        OnAdded?.Invoke(this, new AddedEventArgs<TCloudItem>(this));
    }
}
foreach (var child in Children)
{
    child.AddRenderer(renderer);
}
```

## Cloud data types

Cloud data types are:
- can be rendered on scene in Unity,
- implement [ICloudItem](../Assets/Scripts/Data/PackageObjects/ICloudItem.cs) interface,
- are `struct`.

At this moment there are several cloud data types:
- [SlamPoint](../Assets/Scripts/Data/PackageObjects/SlamPoint.cs).
- [SlamLine](../Assets/Scripts/Data/PackageObjects/SlamLine.cs) Line between two SlamPoint.
- [SimpleLine](../Assets/Scripts/Data/PackageObjects/SimpleLine.cs) Line between two points in space.
- [SlamObservation](../Assets/Scripts/Data/PackageObjects/SlamObservation.cs) Observation (in other terms key frame).
- [SlamTrackedObject](../Assets/Scripts/Data/PackageObjects/SlamTrackedObject.cs) Tracked object.
- [SlamPlane](../Assets/Scripts/Data/PackageObjects/SlamPlane.cs).

Some of basic containers of cloud data types already implemented in Elektronik:
- [CloudContainerBase\<T\>](../Assets/Scripts/DataSources/Containers/CloudContainerBase.cs) Most simple container.
- [CloudContainer\<T\>](../Assets/Scripts/DataSources/Containers/CloudContainer.cs) Inheritor of `CloudContainerBase` also implements
  `ILookable`, `IVisible`, `ITraceable`, `IClusterable` interfaces.
- [ConnectableObjectsContainer\<T\>](../Assets/Scripts/DataSources/Containers/ConnectableObjectsContainer.cs) Container for objects that are connected to each other.
- [TrackedObjectsContainer](../Assets/Scripts/DataSources/Containers/TrackedObjectsContainer.cs) Contains not only objects themself but also history of their movement.
- [MeshReconstructor](../Assets/Scripts/DataSources/Containers/MeshReconstructor.cs) Builds mesh based on data from other containers.

[<- Usage](Usage-EN.md) | [Write your onw plugin ->](Plugins-EN.md)