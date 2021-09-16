### Архитектура Электроника построена на основе паттрена MVC.

- В качестве модели выступают классы "источники данных" реализующие интерфейс [ISourceTreeNode](../Assets/Scripts/Data/ISourceTreeNode.cs).
- Отображением занимаются "рендереры", класс реализующие интерфейс [ISourceRenderer](../Assets/Scripts/Renderers/ISourceRenderer.cs).
- Класс [DataSourcesController](../Assets/Scripts/Data/DataSourcesController.cs) осуществляет подписку рендереров на изменения модели.

![Диаграмма](Images/ElektronikDiagram.png)

## Модель

Источники данных можно разделить на три категории: 
- Контейнеры облачных данных. Они содержат данные которые можно нарисовать на сцене (точки, линии, плоскости, меши, наблюдения и отслеживаемые объекты). 
  Эти контейнеры также реализуют интерфейс [IContainer\<T\>](../Assets/Scripts/Containers/IContainer.cs). 
  Референсная реализация приведена в классе [CloudContainerBase\<T\>](../Assets/Scripts/Containers/CloudContainerBase.cs).
- Виртуальные. Они не содержат информации, а лишь позволяют организовать другие источники в дерево. Реализуются классом [VirtualSource](../Assets/Scripts/Containers/VirtualSource.cs) 
- Все остальные. Они содержат данные, которые нельзя нарисовать на сцене, например текст, изображения с камеры и т.д. 
  Эти источники также реализуют интерфейс [IRendersToWindow](../Assets/Scripts/UI/Windows/IRendersToWindow.cs)

Источники данных могут реализовывать дополнительные интерфейсы, которые позволяют пользователю взаимодействовать с ними через окно "Дерево данных":
- ![button](Images/VisibilityButton.png) [IVisible](../Assets/Scripts/Containers/SpecialInterfaces/IVisible.cs) Позволяет включать или выключать отображение источника.
  *Посылает в рендер сигнал о удалении всех объектов при выключении видимости и о добавлении - при включении.* Референсная реализация: [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs).
- ![button](Images/LookAtButton.png) [ILookable](../Assets/Scripts/Containers/SpecialInterfaces/ILookable.cs) Позволяет навести камеру на данные.
  *Для переданных координат камеры возвращает ближайшие координаты с которых будет видно всё содержимое контейнера* Референсная реализация: [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs).
- ![button](Images/LookAtButton.png) [IFollowable](../Assets/Scripts/Containers/SpecialInterfaces/IFollowable.cs) Позволяет закрепить камеру за объектом.
  Референсная реализация: [TrackContainer](../Assets/Scripts/Containers/TrackContainer.cs).
- ![button](Images/DeleteButton.png) [IRemovable](../Assets/Scripts/Containers/SpecialInterfaces/IRemovable.cs) Позволяет удалить источник данных из дерева. 
  Референсная реализация: [CloudContainer\<T\>](../Assets/Scripts/Clusterization/ClustersContainer.cs).
- ![button](Images/SaveButton.png) [ISave](../Assets/Scripts/Containers/SpecialInterfaces/ISave.cs) Маркирует, что источник можно сохранять в файл.
- ![button](Images/TraceButton.png) [ITraceable](../Assets/Scripts/Containers/SpecialInterfaces/ITraceable.cs) Маркирует, что для данных из этого источника можно рисовать след.
  Референсная реализация: [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs).
- ![button](Images/Connections.png) [IWeightable](../Assets/Scripts/Containers/SpecialInterfaces/IWeightable.cs) Позволяет управлять отображением данных в зависимости от числового коэффициента.
  *Посылает в рендер сигнал о удалении всех объектов не соответствующих коэффициенту.* Референсная реализация: [Connector](../Assets/Scripts/Containers/Connector.cs).
- [ISnapshotable](../Assets/Scripts/Containers/SpecialInterfaces/ISnapshotable.cs) Позволяет скопировать содержимое контейнера и его дочерних элементов. 
  Референсная реализация: [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs).

Структура модели, добавление и изменение данных в ней определяется в [плагинах](Plugins-RU.md).

## Отображение

Рендереры можно разделить на две категории:
- Для отображения облачных данных (точки, линии, плоскости, меши, наблюдения и отслеживаемые объекты) на сцене. 
  Они реализуют интерфейс [ICloudRenderer\<T\>](../Assets/Scripts/Clouds/Renderers/ICloudRenderer.cs) или [IMeshRenderer](../Assets/Scripts/Clouds/Renderers/IMeshRenderer.cs).
  Референсная реализация: [CloudRenderer\<T\>](../Assets/Scripts/Clouds/Renderers/CloudRenderer.cs).
- Для отображения всех остальных типов данных в отдельных окнах.
  Они реализуют интерфейс [IDataRenderer\<T\>](../Assets/Scripts/Renderers/IDataRenderer.cs).
  Референсная реализация: [ImageRenderer](../Assets/Scripts/Renderers/ImageRenderer.cs).

## Контроллер

При появлении нового источника данных контроллер передаёт в него все зарегистрированные рендереры через метод `ISourceTree.AddRenderer(ISourceRenderer)`.
Источник должен проверить подходящий ли это рендерер, если да то подписать его на события обновления, после чего предать в дочерние источники.
Рендереры и источники соединены связью многие-ко-многим.
Пример:
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

## Типы облачных данных

К облачным данным могут относиться любые типы, которые:
- могут быть отрендерены на сцене в Unity,
- реализуют интерфейс [ICloudItem](../Assets/Scripts/Data/PackageObjects/ICloudItem.cs),
- являются структурами.

В данный момент реализованы следующие типы данных:
- [SlamPoint](../Assets/Scripts/Data/PackageObjects/SlamPoint.cs) Точка.
- [SlamLine](../Assets/Scripts/Data/PackageObjects/SlamLine.cs) Линия между двумя SlamPoint.
- [SimpleLine](../Assets/Scripts/Data/PackageObjects/SimpleLine.cs) Линия между двумя точками без привязки к SlamPoint.
- [SlamObservation](../Assets/Scripts/Data/PackageObjects/SlamObservation.cs) Наблюдение (в другой терминологии ключевой кадр).
- [SlamTrackedObject](../Assets/Scripts/Data/PackageObjects/SlamTrackedObject.cs) Отслеживаемый объект.
- [SlamPlane](../Assets/Scripts/Data/PackageObjects/SlamPlane.cs) Плоскости.

Также для удобства разработки плагинов в Электронике уже реализованы некоторые контейнеры для хранения облачных типов данных:
- [CloudContainerBase\<T\>](../Assets/Scripts/Containers/CloudContainerBase.cs) Простой контейнер данных.
- [CloudContainer\<T\>](../Assets/Scripts/Containers/CloudContainer.cs) Наследник `CloudContainerBase`, который реализует ещё и
  `ILookable`, `IVisible`, `ITraceable`, `IClusterable`, `ISnapshotable`.
- [ConnectableObjectsContainer\<T\>](../Assets/Scripts/Containers/ConnectableObjectsContainer.cs) Контейнер для объектов, которые связаны между собой.
- [TrackedObjectsContainer](../Assets/Scripts/Containers/TrackedObjectsContainer.cs) Контейнер, который хранит не только сами объекты, но ещё и историю их перемещения.
- [MeshReconstructor](../Assets/Scripts/Containers/MeshReconstructor.cs) Контейнер, который по содержимому дркгих контейнеров строит меш.

[<- Использование](Usage-RU.md) | [Написать свой плагин ->](Plugins-RU.md)