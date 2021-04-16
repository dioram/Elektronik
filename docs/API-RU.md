# Обработчики входящих данных

## [IChainable\<T\>](../Assets/Scripts/IChainable.cs)

Интерфейс для обработчиков в паттерне "Цепочка обязанностей".

Свойства и методы:
```c#
/// <summary> Sets next command to perform after this. </summary>
/// <param name="link"> Next command. </param>
/// <returns> Successor. </returns>
IChainable<T> SetSuccessor(IChainable<T> link);
```

## [ICommand](../Assets/Scripts/Commands/ICommand.cs)

Интерфейс для комманд в паттерне "Цепочка команд".

Наследники реализуют поведение для обработки конкретного типа команд.

**Примеры реализации** [AddCommand](../Assets/Scripts/Commands/Generic/AddCommand.cs),
[ClearCommand](../Assets/Scripts/Commands/Generic/ClearCommand.cs)

Свойства и методы:
```c#
/// <summary> Executes this command. </summary>
void Execute();

/// <summary> Undo this command. </summary>
void UnExecute();
```

## [ICSConverter](../Assets/Scripts/Data/Converters/ICSConverter.cs)

Дает возможность настраивать переход между системами координат для различных видов пакетов.

**Пример реализации:** [Camera2Unity3dPackageConverter](../Assets/Scripts/Data/Converters/Camera2Unity3dPackageConverter.cs).

# Структуры для хранения данных

## [ICloudItem\<T\>](../Assets/Scripts/Data/PackageObjects/ICloudItem.cs)

Интерфейс для хранения данных об одном объекте.

**Примеры реализации** [SlamPoint](../Assets/Scripts/Data/PackageObjects/SlamPoint.cs),
[SlamObservation](../Assets/Scripts/Data/PackageObjects/SlamObservation.cs)

Свойства и методы:
```c#
/// <summary> Unique ID for item </summary>
int Id { get; set; }

/// <summary> Additional info. </summary>
string Message { get; set; }
```

## [IContainer\<T\>](../Assets/Scripts/Containers/IContainer.cs)

Интерфейс контейнера для хранения объектов с возможностью группового добавления, обновления и удаления.
Также предоставляет возможность подписаться на события добавления, обновления и удаления.

**Примеры реализации** [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs),
[TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs)

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

## [IConnectableObjectsContainer\<T\>](../Assets/Scripts/Containers/IConnectableObjectsContainer.cs)

Интерфейс контейнера для хранения объектов, у которых могут быть установлены соединения друг с другом.

**Пример реализации** [ConnectableObjectContainer\<T\>](../Assets/Scripts/Containers/ConnectableObjectsContainer.cs)

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

## [IContainerTree](../Assets/Scripts/Containers/IContainerTree.cs)

Интерфейс для контейнеров, которые могут соединяться в древовидную структуру.

**Примеры реализации** [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs),
[TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs)

Свойства и методы:
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

Интерфейс для контейнеров на содержимое которых можно навести камеру.

**Примеры реализации** [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs),
[TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs)

Свойства и методы:
```c#
/// <summary> Returns coordinates of camera for best position to look at container's content. </summary>
/// <param name="transform"> Initial camera transform. </param>
/// <returns> End camera transform. </returns>
(Vector3 pos, Quaternion rot) Look(Transform transform);
```

# Классы для отображения данных

## [CloudRenderer\<TCloudItem, TCloudBlock\>](../Assets/Scripts/Clouds/CloudRenderer.cs)

Базовый класс для отображения облаков объектов, таких как точки, линии и т.д.
В целях оптимизации группирует объекты [поблочно](#CloudBlock).

**Примеры реализации** [PointCloudRenderer](../Assets/Scripts/Clouds/PointCloudRenderer.cs),
[LineCLoudRenderer](../Assets/Scripts/Clouds/LineCloudRenderer.cs)

Свойства и методы:
```c#
public Shader CloudShader;
public float ItemSize;
public void SetSize(float newSize);
public void OnItemsAdded(IContainer<TCloudItem> sender, AddedEventArgs<TCloudItem> e);
public void OnItemsUpdated(IContainer<TCloudItem> sender, UpdatedEventArgs<TCloudItem> e);
public void OnItemsRemoved(IContainer<TCloudItem> sender, RemovedEventArgs e);
```

## [CloudBlock](../Assets/Scripts/Clouds/CloudBlock.cs)

Базовый класс для рендера блока объектов.

**Примеры реализации** [PointCloudBlock](../Assets/Scripts/Clouds/PointCloudBlock.cs),
[LineCLoudBlock](../Assets/Scripts/Clouds/LineCloudBlock.cs)

[<- Формат обмена данными](Usage-RU.md) | [Написать свой плагин ->](Plugins-RU.md)