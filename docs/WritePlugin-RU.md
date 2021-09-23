# Написать свой плагин

В данный момент Электроник поддерживает плагины:
- добавляющие новые типы источников данных, например протоколы передачи данных или форматы файлов;
- позволяющие записывать состояние сцены;
- добавляющие алгоритмы разбиения облака точек на кластеры.

## Введение

Ваш плагин может быть написан на любом языке поддерживаемом средой .Net.
Для того чтобы плагин имел доступ к данным электроника, его сборка должна ссылаться на
файлы `UnityEngine.dll`, `Assembly-CSharp.dll`, `Assembly-CSharp-firstpass.dll`.
Эти файлы поставляются вместе с Электроником и находятся в директории `<YOUR_ELEKTRONIK_DIR>\Elektronik_Data\Managed`.
Также вам возможно понадобится сослаться на другие сборки движка Unity.
Например класс `Vector3` находится в `UnityEngine.CoreModule.dll`.

Плагин должен быть написан с использованием .netstandard2.0 в качестве Target framework.

Ваш плагин должен содержать классы реализующие хотябы один интерфейс фабрики для плагинов и соответствующий ей 
интрефейс плагина.

Плагин должен представлять собой dll библиотеку и дополнительные файлы.
При запуске электроник будет искать плагины в директории `<YOUR_ELEKTRONIK_DIR>\Plugins`.
Ваш плагин должен иметь следующую структуру:
```
\Plugins
  \<your_plugin_name>
    \libraries
      \your_plugin.dll
      \additional_libs.dll
    \data
      \translations.csv
      \<plugin_display_name>_Logo.png
      \some_other_data.csv
```

## Общее для всех типов плагинов
- Фабрики: 
  - Общий интерфейс фабрики [IElektronikPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IElektronikPluginsFactory.cs)
  - Абстрактный класс фабрики [ElektronikPluginsFactoryBase\<TSettings\>](../Assets/Scripts/PluginsSystem/Factories/ElektronikPluginsFactoryBase.cs)
  Данный класс берет на себя рутинные операции по работе с настройками, загрузке логотипа и создании плагина.
  Сложно представить ситуацию, когда вам будет выгоднее реализовать `IElektronikPluginsFactory`, чем `ElektronikPluginsFactoryBase`.
  В качестве generic-типа используется тот тип с настройками которые нужны для инициализации вашего плагина. 
- Плагины:
  - Общий интерфейс плагина [IElektronikPlugin](../Assets/Scripts/PluginsSystem/Plugins/IElektronikPlugin.cs)
    - Данный интерфейс также требует реализации паттерна `IDisposable`.

## Плагины источники данных
- Фабрики:
  - [IDataSourcePluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataSourcePluginsFactory.cs)
    Определяет фабрику для любых плагинов - источников данных. 
    Референсная реализация: [ProtobufOnlinePlayerFactory](../Plugins/Protobuf/Online/ProtobufOnlinePlayerFactory.cs)
  - [ISnapshotReaderPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataSourcePluginsFactory.cs)
    Наследник [IDataSourcePluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataSourcePluginsFactory.cs).
    Определяет фабрику создающую плагины, которые могут читать файлы со снапшотами.
    Референсная реализация: [ProtobufFilePlayerFactory](../Plugins/Protobuf/Offline/ProtobufFilePlayerFactory.cs)
- Плагины:
  - [IDataSourcePlugin](../Assets/Scripts/PluginsSystem/Plugins/IDataSourcePlugin.cs)
    Интерфейс для любых плагинов - источников данных.
    Референсная реализация: [ProtobufOnlinePlayer](../Plugins/Protobuf/Online/ProtobufOnlinePlayer.cs)

## Плагины - потребители данных
- Фабрики:
  - [ICustomRecorderPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataRecorderPluginsFactory.cs)
    Интерфейс фабрики для любых плагинов - потребителей данных.
    Референсная реализация: [ProtobufRetranslatorFactory](../Plugins/Protobuf/Recorders/ProtobufRetranslatorFactory.cs)
  - [IFileRecorderPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataRecorderPluginsFactory.cs)
    Интерфейс фабрики для плагинов записывающих данные в файл.
    Нужен для того, чтобы удобно сгрупировать плагины этого типа в пользовательском интерфейсе.
    Референсная реализация: [ProtobufRecorderFactory](../Plugins/Protobuf/Recorders/ProtobufRecorderFactory.cs)
- Плагины:
  - [IDataRecorderPlugin](../Assets/Scripts/PluginsSystem/Plugins/IDataRecorderPlugin.cs)
    Интерфейс для любых плагинов - потребителей данных. Не рекомендуется реализовывать этот интерфейс напрямую,
    лучше отнаследоваться от одного из следующих абстрактных классов.
  - [DataRecorderPluginBase](../Assets/Scripts/PluginsSystem/Plugins/DataRecorderPluginBase.cs)
    Абстрактный класс от которого рекомендуется наследовать все плагины - потребители данных.
    Референсная реализация: [ProtobufRetranslator](../Plugins/Protobuf/Recorders/ProtobufRetranslator.cs)
  - [FileRecorderPluginBase](../Assets/Scripts/PluginsSystem/Plugins/DataRecorderPluginBase.cs)
    Абстрактный класс от которого рекомендуется наследовать те плагины - потребители данных, 
    которые просто пишут состояние сцены в файл.
    Референсная реализация: [ProtobufRecorder](../Plugins/Protobuf/Recorders/ProtobufRecorder.cs)

## Алгоритмы сегментации облаков точек.
  - Фабрика: [IClusteringAlgorithmFactory](../Assets/Scripts/PluginsSystem/Factories/IClusteringAlgorithmFactory.cs)
    Референсная реализация: [KMeansClusteringFactory](../Plugins/Clustering.KMeans/KMeansClusteringFactory.cs)
  - Плагины:
    - [IClusteringAlgorithm](../Assets/Scripts/PluginsSystem/Plugins/IClusteringAlgorithm.cs)
      Интерфейс для любых плагинов - алгоритмов сегментации. Не рекомендуется реализовывать этот интерфейс напрямую,
      лучше отнаследоваться от класса
    - [ClusteringAlgorithmBase](../Assets/Scripts/PluginsSystem/Plugins/IClusteringAlgorithm.cs).
      Референсная реализация: [KMeansClusteringAlgorithm](../Plugins/Clustering.KMeans/KMeansClusteringAlgorithm.cs)