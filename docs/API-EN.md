# Incoming data handlers

## [IChainable\<T\>](../Assets/Scripts/IChainable.cs)

Interface for handlers in pattern "Chain of responsibility"

Members:
```c#
/// <summary> Sets next command to perform after this. </summary>
/// <param name="link"> Next command. </param>
/// <returns> Successor. </returns>
IChainable<T> SetSuccessor(IChainable<T> link);
```

## [ICommand](../Assets/Scripts/Commands/ICommand.cs)

Interface for command in pattern "Chain of command"

Inheritors should implement handling specific type of command.

**Implementation examples:** [AddCommand](../Assets/Scripts/Commands/Generic/AddCommand.cs),
[ClearCommand](../Assets/Scripts/Commands/Generic/ClearCommand.cs)

Members:
```c#
/// <summary> Executes this command. </summary>
void Execute();

/// <summary> Undo this command. </summary>
void UnExecute();
```

## [ICSConverter](../Assets/Scripts/Data/Converters/ICSConverter.cs)

Translates data between coordinate systems.

**Implementation example:** [Camera2Unity3dPackageConverter](../Assets/Scripts/Data/Converters/Camera2Unity3dPackageConverter.cs).

# Data structures

## [ICloudItem\<T\>](../Assets/Scripts/Data/PackageObjects/ICloudItem.cs)

Interface for cloud objects.

**Implementation examples:** [SlamPoint](../Assets/Scripts/Data/PackageObjects/SlamPoint.cs),
[SlamObservation](../Assets/Scripts/Data/PackageObjects/SlamObservation.cs)

Members:
```c#
/// <summary> Unique ID for item </summary>
int Id { get; set; }

/// <summary> Additional info. </summary>
string Message { get; set; }
```

## [IContainer\<T\>](../Assets/Scripts/Containers/IContainer.cs)

Interface of container that allows batched adding, updating, and removing of its elements.
Also rises events on adding, updating, and removing.

**Implementation examples:** [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs),
[TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs)

Members:
```c#
public event Action<IContainer<T>, AddedEventArgs<T>> OnAdded;
public event Action<IContainer<T>, UpdatedEventArgs<T>> OnUpdated;
public event Action<IContainer<T>, RemovedEventArgs> OnRemoved;

T this[T obj] { get; set; }
void AddRange(IEnumerable<T> objects);
void Remove(IEnumerable<T> objs);
bool TryGet(T obj, out T current);
void UpdateItem(T obj);
void UpdateItems(IEnumerable<T> objs);
```

## [IContainerTree](../Assets/Scripts/Containers/IContainerTree.cs)

Containers with this interface can be united in tree structure.

**Implementation examples:** [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs),
[TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs)

Members:
```c#
/// <summary> Display name of container. </summary>
/// <remarks> Will be used in ContainerTree UI widget. </remarks>
string DisplayName { get; set; }
IEnumerable<IContainerTree> Children { get; }
/// <summary> Should content of this container be displayed or not. </summary>
bool IsActive { get; set; }
/// <summary> Clear all content. </summary>
void Clear();
/// <summary> Sets renderer class for this container. </summary>
/// <remarks> You should choose and set only correct type of renderer in implementation of this method. </remarks>
/// <example>
/// if (renderer is ICloudRenderer&lt;TCloudItem&gt; typedRenderer)
/// {
///     OnAdded += typedRenderer.OnItemsAdded;
///     OnUpdated += typedRenderer.OnItemsUpdated;
///     OnRemoved += typedRenderer.OnItemsRemoved;
///     if (Count > 0)
///     {
///         OnAdded?.Invoke(this, new AddedEventArgs&lt;TCloudItem&gt;(this));
///     }
/// }
/// </example>
/// <param name="renderer"> Content renderer </param>
void SetRenderer(object renderer);
```

## [ILookable](../Assets/Scripts/Containers/ILookable.cs)

Containers with this interface can give coordinates for best position to look at theirs content.

**Implementation examples:** [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs),
[TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs)

Members:
```c#
/// <summary> Returns coordinates of camera for best position to look at container's content. </summary>
/// <param name="transform"> Initial camera transform. </param>
/// <returns> End camera transform. </returns>
(Vector3 pos, Quaternion rot) Look(Transform transform);
```

## [IConnectableObjectsContainer\<T\>](../Assets/Scripts/Containers/IConnectableObjectsContainer.cs)

Interface of container for connectable objects.

**Implementation example:** [ConnectableObjectContainer\<T\>](../Assets/Scripts/Containers/ConnectableObjectsContainer.cs)

Members:
```c#
IEnumerable<SlamLine> Connections { get; }
bool AddConnection(int id1, int id2);
void AddConnections(IEnumerable<(int id1, int id2)> connections);
bool AddConnection(T obj1, T obj2);
void AddConnections(IEnumerable<(T obj1, T obj2)> connections);
bool RemoveConnection(int id1, int id2);
void RemoveConnections(IEnumerable<(int id1, int id2)> connections);
bool RemoveConnection(T obj1, T obj2);
void RemoveConnections(IEnumerable<(T obj1, T obj2)> connections);
IEnumerable<(int id1, int id2)> GetAllConnections(int id);
IEnumerable<(int id1, int id2)> GetAllConnections(T obj);
```

# Rendering classes

## [CloudRenderer\<TCloudItem, TCloudBlock\>](../Assets/Scripts/Clouds/CloudRenderer.cs)

Base class for rendering object clouds. Such as point cloud, line cloud, etc.
For optimisation reasons groups objects in [blocks](#CloudBlock).

**Implementation examples:** [PointCloudRenderer](../Assets/Scripts/Clouds/PointCloudRenderer.cs),
[LineCLoudRenderer](../Assets/Scripts/Clouds/LineCloudRenderer.cs)

Members:
```c#
public Shader CloudShader;
public float ItemSize;
public void SetSize(float newSize);
public void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e);
public void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e);
public void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e);
```

## [CloudBlock](../Assets/Scripts/Clouds/CloudBlock.cs)

Base class for rendering block of cloud objects.

**Implementation examples:** [PointCloudBlock](../Assets/Scripts/Clouds/PointCloudBlock.cs),
[LineCLoudBlock](../Assets/Scripts/Clouds/LineCloudBlock.cs)

[<- Usage](Usage-EN.md) | [Write your own plugin ->](Plugins-EN.md)