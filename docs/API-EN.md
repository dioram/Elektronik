# Incoming data handlers

## [IChainable\<T\>](../Assets/Scripts/Common/IChainable.cs)

Interface for handlers in pattern "Chain of responsibility"

Members:
```c#
/// <summary> Sets next command to perform after this. </summary>
/// <param name="link"> Next command. </param>
/// <returns> Successor. </returns>
IChainable<T> SetSuccessor(IChainable<T> link);
```

## [MapManager\<T\>](../Assets/Scripts/Online/GrpcServices/MapManager.cs)

Base class for handle data in online mode. Used in pattern "Chain of responsibility".

Inheritors should implement handling specific types of incoming data.

**Implementation examples:** [PointsMapManager](../Assets/Scripts/Online/GrpcServices/PointsMapManager.cs),
[ObservationsMapManager](../Assets/Scripts/Online/GrpcServices/ObservationsMapManager.cs)

Members:
```c#
/// <summary> Handles gRPC request. </summary>
/// <param name="request"> Packet to handle. </param>
/// <param name="context"> Server call context </param>
/// <returns> Async error status </returns>
public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context);
```

## [Commander](../Assets/Scripts/Common/Commands/Commander.cs)

Base class for handle data in offline mode. Used in pattern "Chain of responsibility".

Inheritors should implement handling specific types of incoming data.

**Implementation examples:** [ObjectsCommander](../Assets/Scripts/Offline/Commanders/ObjectsCommander.cs),
[TrackedObjectsCommander](../Assets/Scripts/Offline/Commanders/TrackedObjectsCommander.cs)

Members:
```c#
/// <summary> Sets converter for this commander and its successors. </summary>
/// <param name="converter"> Converter to set. </param>
public virtual void SetConverter(ICSConverter converter);
    
/// <summary> Extracts commands form packet. </summary>
/// <param name="pkg"> Packet with commands. </param>
/// <param name="commands"> List where new commands will be added to. </param>
public virtual void GetCommands(PacketPb pkg, in LinkedList<ICommand> commands);
```

## [ICommand](../Assets/Scripts/Common/Commands/ICommand.cs)

Interface for command in pattern "Chain of command"

Inheritors should implement handling specific type of command.

**Implementation examples:** [AddCommand](../Assets/Scripts/Common/Commands/Generic/AddCommand.cs),
[ClearCommand](../Assets/Scripts/Common/Commands/Generic/ClearCommand.cs)

Members:
```c#
/// <summary> Executes this command. </summary>
void Execute();

/// <summary> Undo this command. </summary>
void UnExecute();
```

## [ICSConverter](../Assets/Scripts/Common/Data/Converters/ICSConverter.cs)

Translates data between coordinate systems.

**Implementation example:** [Camera2Unity3dPackageConverter](../Assets/Scripts/Common/Data/Converters/Camera2Unity3dPackageConverter.cs).

# Data structures

## [ICloudItem\<T\>](../Assets/Scripts/Common/Data/PackageObjects/ICloudItem.cs)

Interface for cloud objects.

**Implementation examples:** [SlamPoint](../Assets/Scripts/Common/Data/PackageObjects/SlamPoint.cs),
[SlamObservation](../Assets/Scripts/Common/Data/PackageObjects/SlamObservation.cs)

Members:
```c#
/// <summary> Unique ID for item </summary>
int Id { get; set; }

/// <summary> Additional info. </summary>
string Message { get; set; }
```

## [IContainer\<T\>](../Assets/Scripts/Common/Containers/IContainer.cs)

Interface of container that allows batched adding, updating, and removing of its elements.
Also rises events on adding, updating, and removing.

**Implementation examples:** [SlamPointContainer](../Assets/Scripts/Common/Containers/SlamPointsContainer.cs),
[SlamObservationContainer](../Assets/Scripts/Common/Containers/SlamObservationsContainer.cs)

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

## [IConnectableObjectsContainer\<T\>](../Assets/Scripts/Common/Containers/Connectable/IConnectableObjectsContainer.cs)

Interface of container for connectable objects.

**Implementation examples:** [ConnectablePointsContainer](../Assets/Scripts/Common/Containers/Connectable/ConnectablePointsContainer.cs),
[ConnectableObservationContainer](../Assets/Scripts/Common/Containers/Connectable/ConnectableObjectsContainer.cs)

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

## [CloudRenderer\<TCloudItem, TCloudBlock\>](../Assets/Scripts/Common/Clouds/CloudRenderer.cs)

Base class for rendering object clouds. Such as point cloud, line cloud, etc.
For optimisation reasons groups objects in [blocks](#CloudBlock).

**Implementation examples:** [PointCloudRenderer](../Assets/Scripts/Common/Clouds/PointCloudRenderer.cs),
[LineCLoudRenderer](../Assets/Scripts/Common/Clouds/LineCloudRenderer.cs)

Members:
```c#
public Shader CloudShader;
public float ItemSize;
public void SetSize(float newSize);
public void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e);
public void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e);
public void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e);
```

## [CloudBlock](../Assets/Scripts/Common/Clouds/CloudBlock.cs)

Base class for rendering block of cloud objects.

**Implementation examples:** [PointCloudBlock](../Assets/Scripts/Common/Clouds/PointCloudBlock.cs),
[LineCLoudBlock](../Assets/Scripts/Common/Clouds/LineCloudBlock.cs)

# Online and Offline classes

## [Server](../Assets/Scripts/Online/Server.cs)

Class that launches gRPC server and init chain of responsibility for handling incoming data.

## [SlamEventsManager](../Assets/Scripts/Offline/SlamEventsManager.cs)

Class, that controls chain of commands in offline mode.

[<- Data transfer format](Data-EN.md) | [Back to start page](Home-EN.md)