# Write your own plugin

For now Elektronik supports only plugins for new data sources such as data transfer protocols or file formats.

If you need protocol or format not supported by Elektronik this page will help you write
your own plugin.

## Introduction

You can use any language supported by .Net.
Your plugin need to reference files `UnityEngine.dll`, `Assembly-CSharp.dll`, `Assembly-CSharp-firstpass.dll` 
from `<YOUR_ELEKTRONIK_DIR>\Elektronik_Data\Managed`.
Also you maybe have to reference other Unity dll.
For `Vector3` you should reference `UnityEngine.CoreModule.dll`.

Plugin should contains classes implements one of these interfaces:
- [IDataSourceOnline](../Assets/Scripts/PluginsSystem/IDataSourceOnline.cs) for streaming data.
- [IDataSourceOffline](../Assets/Scripts/PluginsSystem/IDataSourceOffline.cs) for reading files 
  or any other data which can be stopped or rewinded.

In offline mode elektronik supports only one active data source.

## Common

You need to implement these functions and properties:
```c#
/// <summary> Name to display in plugins settings. </summary>
string DisplayName { get; }

/// <summary> Plugins description. Will be displayed in plugins settings </summary>
string Description { get; }
```
- Just name and description to be displayed in settings menu.
```c#
/// <summary> Converter for raw Vector3 and Quaternions </summary>
ICSConverter Converter { get; set; }
```
- At plugin here will be written convertor form right-handed to left-handed coordinate system.
  You can use it or implement your own.
```c#
/// <summary> Containers with cloud data. </summary>
IContainerTree Data { get; }
```
- Root of cloud containers tree. In Elektronik all data about cloud objects (points, lines, planes, etc.) are placed
  in containers and containers implement a tree structure. More information [here](#Cloud-objects-containers).
  This property should be initialized statically or in constructor.
```c#
/// <summary> Containers with any data. </summary>
DataPresenter PresentersChain { get; }
```
- Chain of containers for not cloud data such as images or text. More information [here](#Other-types-of-data).
  This property should be initialized statically or in constructor.
```c#
/// <summary> Starts plugin. </summary>
void Start();
```
- Function where you should start your plugin. This function can be call several times at program execution.
  `Stop()` would be called before any new call of `Start()` (except first time).
```c#
/// <summary> Stops plugin. </summary>
void Stop();
```
- Function whwre you sholud stop your plugin and clear all it internal data and state. 
  This function can be call several times at program execution.
```c#
/// <summary> Calls every time when Unity.Update() event happens. </summary>
/// <param name="delta"> Time from previous update call in seconds. </param>
void Update(float delta);
```
- This functions will be called each time unity render a new frame. You can use it as you wish, but don't do long operations in here.
```c#
/// <summary> Plugins settings. </summary>
SettingsBag Settings { get; set; }
```
- Plugins settings object. For example it can contains path to file to open or porn for data listening.
  More information [here](#Settings).
```c#
/// <summary> Container for settings history. </summary>
ISettingsHistory SettingsHistory { get; }
```
- Object that keeping plugin's settings history. Most likely you will not use it.
  But it should be initialized statically or in constructor of pluing.
  More information [here](#Settings).

## Online mode

Create a class implementing [IDataSourceOnline](../Assets/Scripts/PluginsSystem/IDataSourceOnline.cs)
to implement online mode. There is nothing to implement here except common functions and properties.

## Offline mode

Create a class implementing [IDataSourceOffline](../Assets/Scripts/PluginsSystem/IDataSourceOffline.cs)
to implement offline mode.
```c#
public interface IDataSourceOffline : IDataSource
{
    /// <summary> Amount of frames (commands) in file. </summary>
    int AmountOfFrames { get; }
    
    /// <summary> Displaying timestamp of current frame. </summary>
    int CurrentTimestamp { get; }
    
    /// <summary> Number of current frame. </summary>
    int CurrentPosition { get; set; }
    
    /// <summary> Play button pressed handler. </summary>
    void Play();
    
    /// <summary> Next button pressed handler. </summary>
    void Pause();
    
    /// <summary> Stop button pressed handler. </summary>
    void StopPlaying();
    
    /// <summary> Previous frame button pressed handler. </summary>
    void PreviousKeyFrame();
    
    /// <summary> Next frame button pressed handler. </summary>
    void NextKeyFrame();

    /// <summary> Reached end of the file. </summary>
    event Action Finished;
}
```
Here you have to implement functions for handling control buttons and properties for displaying state of player.

## Plugins loading

Your plugin must be dll library with any additional files.
Electronik will search in `<YOUR_ELEKTRONIK_DIR>\Plugins` for any plugins.
Plugin must have following structure:
```
\Plugins
  \your_plugin_name
    \libraries
      \your_plugin.dll
      \additional_libs.dll
    \data
      \some_optional_data.csv
```
In settings menu user will select plugins to activate and settings for those plugins.
Playing can be started only after at least one data source is choosed and its settings are correct.

## Cloud data containers

[IContainer\<T\>](../Assets/Scripts/Containers/IContainer.cs) is using for containing cloud objects.
You can write container for your own data or use one of [standard](../Assets/Scripts/Containers). 
For now Elektronk can render only:
- points clouds (T=[SlamPoint](../Assets/Scripts/Data/PackageObjects/SlamPoint.cs)),
- lines clouds (T=[SlamLine](../Assets/Scripts/Data/PackageObjects/SlamLine.cs)),
- infinite planes clouds (T=[SlamInfinitePlane](../Assets/Scripts/Data/PackageObjects/SlamInfinitePlane.cs)),
- observation graph (T=[SlamObservation](../Assets/Scripts/Data/PackageObjects/SlamObservation.cs)),
- and objects with track (T=[SlamTrackedObject](../Assets/Scripts/Data/PackageObjects/SlamTrackedObject.cs)).

For rendering other types of cloud data convert them to one of types above or implement your on 
[cloud renderer](API-RU.md#Rendering-classes).

Containers should implement [IContainerTree](../Assets/Scripts/Containers/IContainerTree.cs)
for grouping in tree structure. Before playing started all cloud renderers will be set for containers using function
`Data.SetRenderer(renderer)`. If you are implementing your own type of container you should subscribe renderer on container updates.
Like here:
```c#
public void SetRenderer(object renderer)
{
  if (renderer is ICloudRenderer<SlamLine> typedRenderer)
  {
      OnAdded += typedRenderer.OnItemsAdded;
      OnUpdated += typedRenderer.OnItemsUpdated;
      OnRemoved += typedRenderer.OnItemsRemoved;
      if (Count > 0)
      {
          OnAdded?.Invoke(this, new AddedEventArgs<SlamLine>(this));
      }
  }
  foreach (var child in Children)
  {
      child.SetRenderer(renderer)
  }
}
```
If you are planning to use several containers then you need ot create root container for all others.
You can find [example](../plugins/Protobuf/Data/ProtobufContainerTree.cs) in Protobuf plugin.

## Other types of data

If you need to present other types of data such as camera images or some text use class
[DataPresenter](../Assets/Scripts/Presenters/DataPresenter.cs).
This class implements pattern chain of responsibility and should parse incoming data and sent it to UI for rendering.
Before playing started all renderers will be set for presenters using function
`PresentersChain.SetRenderer(renderer)`. If you are implementing your own type of presenter you should subscribe renderer on its updates.
Like here:
```c#
if (dataRenderer is IDataRenderer<byte[]> renderer)
{
    _renderer = renderer;
}
Successor?.SetRenderer(dataRenderer);
```

## Settings

Create class inherited from [SettingsBag](../Assets/Scripts/Settings/SettingsBag.cs) for setting of you plugin.
This class also should have `Serializable` attribute.
All public fields of this class will be displayed in settings menu.
If you not want to show some of fields, mark them with `NowShow` atribute.
Mark fields with `Tooltip(string)` attribute to change displaying name of this field.

If you need to validate settings override `bool Validate()` it should return true if settings are correct.
Playing will not started before all turned on plugins have correct settings.

For keeping history of setting create a specialization of class [SettingsHistory\<T\>](../Assets/Scripts/Settings/SettingsHistory.cs)
for your setting class.

Initialize properties
```c#
/// <summary> Plugins settings. </summary>
SettingsBag Settings { get; set; }
/// <summary> Container for settings history. </summary>
ISettingsHistory SettingsHistory { get; }
```
in your plugin statically or in constructor using created classes.

All settings are stored in `C:\Users\<User>\AppData\LocalLow\Dioram\Elektronik\<FullNameOfSettingsClass>.json`.

[<- Internal API](API-EN.md) | [Protobuf plugin ->](Protobuf-EN.md)