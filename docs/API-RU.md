# Обработчики входящих данных

## [IChainable\<T\>](../Assets/Scripts/Common/IChainable.cs)

Интерфейс для обработчиков в паттерне "Цепочка обязанностей".

Свойства и методы:
```c#
/// <summary> Sets next command to perform after this. </summary>
/// <param name="link"> Next command. </param>
/// <returns> Successor. </returns>
IChainable<T> SetSuccessor(IChainable<T> link);
```

## [MapManager\<T\>](../Assets/Scripts/Online/GrpcServices/MapManager.cs)

Обрабатывает данные в онлайн режиме. Используется как обработчик в паттерне "Цепочка обязанностей".

Наследники реализуют поведение для обработки конкретного типа входящих данных.

**Примеры реализации** [PointsMapManager](../Assets/Scripts/Online/GrpcServices/PointsMapManager.cs),
[ObservationsMapManager](../Assets/Scripts/Online/GrpcServices/ObservationsMapManager.cs)

Свойства и методы:
```c#
/// <summary> Handles gRPC request. </summary>
/// <param name="request"> Packet to handle. </param>
/// <param name="context"> Server call context </param>
/// <returns> Async error status </returns>
public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context);
```

## [Commander](../Assets/Scripts/Common/Commands/Commander.cs)

Обрабатывает данные в оффлайн режиме. Используется как обработчик в паттерне "Цепочка обязанностей".

Наследники реализуют поведение для обработки конкретного типа входящих данных.

**Примеры реализации** [ObjectsCommander](../Assets/Scripts/Offline/Commanders/ObjectsCommander.cs),
[TrackedObjectsCommander](../Assets/Scripts/Offline/Commanders/TrackedObjectsCommander.cs)

Свойства и методы:
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

Интерфейс для комманд в паттерне "Цепочка команд".

Наследники реализуют поведение для обработки конкретного типа команд.

**Примеры реализации** [AddCommand](../Assets/Scripts/Common/Commands/Generic/AddCommand.cs),
[ClearCommand](../Assets/Scripts/Common/Commands/Generic/ClearCommand.cs)

Свойства и методы:
```c#
/// <summary> Executes this command. </summary>
void Execute();

/// <summary> Undo this command. </summary>
void UnExecute();
```

## [ICSConverter](../Assets/Scripts/Common/Data/Converters/ICSConverter.cs)

Дает возможность настраивать переход между системами координат из полученных в отображаемые для различных видов пакетов.

**Пример реализации:** [Camera2Unity3dPackageConverter](../Assets/Scripts/Common/Data/Converters/Camera2Unity3dPackageConverter.cs).

# Структуры для хранения данных

## [ICloudItem\<T\>](../Assets/Scripts/Common/Data/PackageObjects/ICloudItem.cs)

Интерфейс для хранения данных об одном объекте.

**Примеры реализации** [SlamPoint](../Assets/Scripts/Common/Data/PackageObjects/SlamPoint.cs),
[SlamObservation](../Assets/Scripts/Common/Data/PackageObjects/SlamObservation.cs)

Свойства и методы:
```c#
/// <summary> Unique ID for item </summary>
int Id { get; set; }

/// <summary> Additional info. </summary>
string Message { get; set; }
```

## [IContainer\<T\>](../Assets/Scripts/Common/Containers/IContainer.cs)

Интерфейс контейнера для хранения объектов с возможностью группового добавления, обновления и удаления.
Также предоставляет возможность подписаться на события добавления, обновления и удаления.

**Примеры реализации** [SlamPointContainer](../Assets/Scripts/Common/Containers/SlamPointsContainer.cs),
[SlamObservationContainer](../Assets/Scripts/Common/Containers/SlamObservationsContainer.cs)

Свойства и методы:
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

Интерфейс контейнера для хранения объектов, у которых могут быть установлены соединения друг с другом.

**Примеры реализации** [ConnectablePointsContainer](../Assets/Scripts/Common/Containers/Connectable/ConnectablePointsContainer.cs),
[ConnectableObservationContainer](../Assets/Scripts/Common/Containers/Connectable/ConnectableObjectsContainer.cs)

Свойства и методы:
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

# Классы для отображения данных

## [CloudRenderer\<TCloudItem, TCloudBlock\>](../Assets/Scripts/Common/Clouds/CloudRenderer.cs)

Базовый класс для отображения облаков объектов, таких как точки, линии и т.д.
В целях оптимизации группирует объекты [поблочно](#CloudBlock).

**Примеры реализации** [PointCloudRenderer](../Assets/Scripts/Common/Clouds/PointCloudRenderer.cs),
[LineCLoudRenderer](../Assets/Scripts/Common/Clouds/LineCloudRenderer.cs)

Свойства и методы:
```c#
public Shader CloudShader;
public float ItemSize;
public void SetSize(float newSize);
public void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e);
public void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e);
public void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e);
```

## [CloudBlock](../Assets/Scripts/Common/Clouds/CloudBlock.cs)

Базовый класс для рендера блока объектов.

**Примеры реализации** [PointCloudBlock](../Assets/Scripts/Common/Clouds/PointCloudBlock.cs),
[LineCLoudBlock](../Assets/Scripts/Common/Clouds/LineCloudBlock.cs)

# Классы онлайн и оффлайн режимов

## [Server](../Assets/Scripts/Online/Server.cs)

Класс, который в онлайн режиме запускает gRPC сервер, а также инициализирует цепочку обязанностей для обработки входящих данных.

## [SlamEventsManager](../Assets/Scripts/Offline/SlamEventsManager.cs)

Класс, который в оффлайн режиме управляет цепочкой комманд.

[<- Формат обмена данными](Data-RU.md) | [К стартовой странице](Home-RU.md)