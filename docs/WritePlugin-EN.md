# Write your own plugin

At this moment Elektronik supports several types of plugins:
- data sources. They can be used to add new protocol or file format support.
- data recorders. They can be used to write scene state to file or send over network.
- algorithms for clustering points cloud.


## Introduction

You can use any language supported by .Net.
Your plugin need to reference files `UnityEngine.dll`, `Assembly-CSharp.dll`, `Assembly-CSharp-firstpass.dll`
from `<YOUR_ELEKTRONIK_DIR>\Elektronik_Data\Managed`.
Also you maybe have to reference other Unity dll.
For `Vector3` you should reference `UnityEngine.CoreModule.dll`.

Plugin must:
- be written using .netstandard2.0 as target framework,
- contain at least one class implemented factory interface and one class implemented plugin interface.
- have this file structure
```
\<YOUR_ELEKTRONIK_DIR>
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

## Common for all plugins
- Factories:
  - Common factory interface [IElektronikPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IElektronikPluginsFactory.cs)
  - Abstract factory class [ElektronikPluginsFactoryBase\<TSettings\>](../Assets/Scripts/PluginsSystem/Factories/ElektronikPluginsFactoryBase.cs)
    This class implements routine operations with settings, logo loading and starting plugin.
    You should always inherit from this class instead of implementing `IElektronikPluginsFactory`.
    It uses type of settings for your plugin as generic type.
- Plugins:
  - Common interface [IElektronikPlugin](../Assets/Scripts/PluginsSystem/Plugins/IElektronikPlugin.cs)
    - This interface requires implementing `IDisposable` pattern.

## Data source plugins
- Factories:
  - [IDataSourcePluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataSourcePluginsFactory.cs)
    Interface for any factory of data source plugins.
    Reference implementation: [ProtobufOnlinePlayerFactory](../Plugins/Protobuf/Online/ProtobufOnlinePlayerFactory.cs)
  - [ISnapshotReaderPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataSourcePluginsFactory.cs)
    Inherits [IDataSourcePluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataSourcePluginsFactory.cs).
  - Interface for factory of plugins that can read files with snapshots.
    Reference implementation: [ProtobufFilePlayerFactory](../Plugins/Protobuf/Offline/ProtobufFilePlayerFactory.cs)
- Plugins:
  - [IDataSourcePlugin](../Assets/Scripts/PluginsSystem/Plugins/IDataSourcePlugin.cs)
    Interface for any data source plugins.
    Reference implementation: [ProtobufOnlinePlayer](../Plugins/Protobuf/Online/ProtobufOnlinePlayer.cs)

## Data recorder (consumer) plugins
- Factories:
  - [ICustomRecorderPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataRecorderPluginsFactory.cs)
    Interface for any factory of data recorder plugins.
    Reference implementation: [ProtobufRetranslatorFactory](../Plugins/Protobuf/Recorders/ProtobufRetranslatorFactory.cs)
  - [IFileRecorderPluginsFactory](../Assets/Scripts/PluginsSystem/Factories/IDataRecorderPluginsFactory.cs)
    Interface for any factory of plugins that record data to file.
    Reference implementation: [ProtobufRecorderFactory](../Plugins/Protobuf/Recorders/ProtobufRecorderFactory.cs)
- Plugins:
  - [IDataRecorderPlugin](../Assets/Scripts/PluginsSystem/Plugins/IDataRecorderPlugin.cs)
    Interface for any data recorder plugins. It's not recommended to implement this interface directly, 
    better inherit from one of next abstract classes.
  - [DataRecorderPluginBase](../Assets/Scripts/PluginsSystem/Plugins/DataRecorderPluginBase.cs)
    Abstract class of any data recorders plugins.
    Reference implementation: [ProtobufRetranslator](../Plugins/Protobuf/Recorders/ProtobufRetranslator.cs)
  - [FileRecorderPluginBase](../Assets/Scripts/PluginsSystem/Plugins/DataRecorderPluginBase.cs)
    Abstract class of data recorders plugins that writes data to file.
    Reference implementation: [ProtobufRecorder](../Plugins/Protobuf/Recorders/ProtobufRecorder.cs)

## Clustering algorithm
- Factory: [IClusteringAlgorithmFactory](../Assets/Scripts/PluginsSystem/Factories/IClusteringAlgorithmFactory.cs)
  Reference implementation: [KMeansClusteringFactory](../Plugins/Clustering.KMeans/KMeansClusteringFactory.cs)
- Plugins:
  - [IClusteringAlgorithm](../Assets/Scripts/PluginsSystem/Plugins/IClusteringAlgorithm.cs)
    Interface for any clustering algorithms. It's not recommended to implement this interface directly,
    better inherit from
  - [ClusteringAlgorithmBase](../Assets/Scripts/PluginsSystem/Plugins/IClusteringAlgorithm.cs).
    Reference implementation: [KMeansClusteringAlgorithm](../Plugins/Clustering.KMeans/KMeansClusteringAlgorithm.cs)
