# Сборка из исходников

Весь процесс сборки из исходников можно поделить на 3 части:
1. Сборка нативных библиотек
2. Сборка Unity проигрывателя
3. Сборка плагинов к Электронику

### Если вы используете `git clone` для получения исходных кодов, то вам потребуется [git-lfs](https://git-lfs.github.com/).
После того, как вы клонировали репозиторий на свой компьютер, перейдите в коренвую директорию репозитория и выполните команду 
```
git lfs fetch --all
```
Это необходимо для того, чтобы все крупные файлы корректно загрузились. 

## Windows

### Сборка нативных библиотек

Некоторые функции электроника не могли быть реализованы в среде .NET и были вынесены в нативный код на языке С++.
К этим функциям относятся:
- Построение меша по облаку точек.
- Выделение плоскостей на облаке точек.
- Получение информации из сети ROS2.

Если вам эти функции не требуются, можете пропустить этот пункт и перейти сразу к сборке Unity проигрывателя.

Для сборки вам потребуются [cmake](https://cmake.org/), [swig](https://swig.org/), 
[Visual Studio 2019](https://visualstudio.microsoft.com/), компилятор c++ (предпочтительно clang), и библиотеки: 
- OpenCV (построение меша и выделение плоскостей),
- PCL (построение меша)
- FastRTPS (ROS2)

Мы настоятельно рекомендуем использовать для их установки пакетный менеджер [vcpkg](https://github.com/microsoft/vcpkg), 
тогда вы сможете установить их одной командой:
```
./vcpkg.exe install openssl opencv fastrtps pcl
```
Для сборки бибилиотеки для получения информации из сети ROS2 перейдите в директорию `plugins/ROS2DDS` и сгенерируйте cmake кеш.
```
cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
```
Если вы используете vcpkg добавьте ключ `CMAKE_TOOLCHAIN_FILE` с путём к файлу `vcpkg.cmake`, например так:
```
-DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake
```
После чего соберите библиотеку:
```
cmake --build .
```
Установите библиотеку в электроник:
```
cmake --install .
```
Аналогичным образом выполняется сборка оставшихся бибилотек. Они находятся в директориях `plugins\MeshBuilder` и `plugins\PlanesDetector`.

### Сборка Unity проигрывателя

Для сборки Unity проигрывателя вам потребуется редактор Unity версии не меньше 2020.3(LTS) и Visual Studio 2019. 
Установите его с помощью оффициального [лаунчера](https://unity3d.com/ru/get-unity/download).

Если вы не собирали библиотеку для построения меша по облаку точек или библиотеку для выделения плоскостей,
найдите в файле `./ProjectSettings/ProjectSettings.asset` строки
```unityyaml
  scriptingDefineSymbols:
    1: 
```
Добавьте туда значение `NO_MESH_BULDER`, если не собиралась библиотека построения меша,
и значение `NO_PLANES_DETECTION`, если не собиралась библиотека выделения плоскостей.
Пример:
```unityyaml
  scriptingDefineSymbols:
    1: NO_MESH_BULDER;NO_PLANES_DETECTION
```

Вы можете собрать проигрыватель из командной строки.
Перейдите в корневую директорию репозитория и запустите следующие команды (путь к unity может отличаться на вашей системе):
```
"C:\Program Files\Unity\Hub\Editor\2020.3.11f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -logFile .\Logs\pre_build.log -executeMethod Elektronik.Editor.PlayerBuildScript.BuildAddressables -projectPath .\
"C:\Program Files\Unity\Hub\Editor\2020.3.11f1\Editor\Unity.exe" -quit -accept-apiupdate -batchmode -logFile .\Logs\build.log -projectPath .\ -buildWindows64Player .\Build\Elektronik.exe 
```
После этого в директории `./build` будет находиться собранный проигрыватель Электроник.

Либо вы можете собрать проигрыватель через интерфейс редактора Unity. 

Перед сборкой нужно будет скомпилировать файлы перевода. Для этого в редакторе Unity выберите меню
`Window->Asset management->Addressables->Gropus` и появившейся вкладке выберите `Build->New build->Default Build Script`.
Дождитесь завершения компиляции и можете собирать проект. Для сборки проекта укажите директорию `./build`, 
иначе для плагинов придётся поменять путь к зависимостям на выбранный вами.

### Сборка плагинов

Если вы не собирали библиотеку для получения информации из сети ROS2, добавьте в файл проекта `./plugins/Ros/Ros.csproj` строку
```xml
<DefineConstants>NO_ROS2DDS</DefineConstants>
```
в разделе `<PropertyGroup>`. Пример:
```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <RootNamespace>Elektronik.RosPlugin</RootNamespace>
    <Deterministic>False</Deterministic>
    <AssemblyVersion>1.2.*</AssemblyVersion>
    <FileVersion>1.2.*</FileVersion>
    <DefineConstants>NO_ROS2DDS</DefineConstants>
</PropertyGroup>
```

Самый простой способ собрать все плагины - это запустить из корневой директории репозитория скрипт 
[build_plugins.bat](../.github/build_plugins.bat). 
Этот скрипт соберёт плагины и установит их в электроник, если он был собран в директории `./build`.

Если же вы хотите собрать плагины вручную, то откройте в Visual Studio файл `./plugins/plugins.sln`, 
внесите все необходимые изменения и соберите всё решение.
После того, как решение было собрано необходимо установить плагины в Электроник.

Для этого нажмите правой клавишей на проект, который вы хотите установить и выберите `Опубликовать...` (`Publish...`).
В открывшемся окне выберите опубликовать в папку. В качестве места публикации выберите 
`<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/libraries/`.
Затем скопиуйте файл переводов `translations.csv` в директорию `<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/data`.

Повторите описанные действия для всех плагинов, которые вы хотите установить.

Поздравляем! Электроник собран и готов к работе.

## Linux

*Данная инструкция по сборке предназначена для дистрибутива Ubuntu 21.04, 
в других дистрибутивах могут отличаться названия пакетов и быть различные несовместимости.*

### Сборка нативных библиотек

Некоторые функции электроника не могли быть реализованы в среде .NET и были вынесены в нативный код на языке С++.
К этим функциям относятся:
- Построение меша по облаку точек.
- Выделение плоскостей на облаке точек.
- Получение информации из сети ROS2.

Если вам эти функции не требуются, можете пропустить этот пункт и перейти сразу к сборке Unity проигрывателя.

Для сборки вам потребуются [cmake](https://cmake.org/), [swig](https://swig.org/), 
компилятор c++ (предпочтительно clang), и библиотеки:
- OpenCV (построение меша и выделение плоскостей),
- PCL (построение меша)
- FastRTPS (ROS2)

Мы рекомендуем использовать для их установки пакетный менеджер [vcpkg](https://github.com/microsoft/vcpkg).
Для того чтобы установить эти библиотеки через vcpkg потребуется установить дополнительные зависимости через apt:
```
sudo apt -y install make swig curl python
```

После чего можно будет запустить установку через vcpkg:
```
./vcpkg.exe install openssl opencv pcl fastrtps
```
Для сборки бибилиотеки для получения информации из сети ROS2 перейдите в директорию `plugins/ROS2DDS` и сгенерируйте cmake кеш.
```
cmake -DCMAKE_CXX_FLAGS=-m64 -DCMAKE_BUILD_TYPE=Release .
```
Если вы используете vcpkg добавьте ключ `CMAKE_TOOLCHAIN_FILE` с путём к файлу `vcpkg.cmake`, например так:
```
-DCMAKE_TOOLCHAIN_FILE=D:\\vcpkg\\scripts\\buildsystems\\vcpkg.cmake
```
После чего соберите библиотеку:
```
cmake --build .
```
Установите библиотеку в электроник:
```
cmake --install .
```
Аналогичным образом выполняется сборка оставшихся бибилотек. Они находятся в директориях `plugins\MeshBuilder` и `plugins\PlanesDetector`.

### Сборка Unity проигрывателя

Для сборки Unity проигрывателя вам потребуется редактор Unity версии не меньше 2020.3(LTS) и среда .NET.
Установите Unity с помощью оффициального [лаунчера](https://unity3d.com/ru/get-unity/download).
Инструкцию по установке .NET вы можете найти на [MSDN](https://docs.microsoft.com/ru-ru/dotnet/core/install/linux).

Если вы не собирали библиотеку для построения меша по облаку точек или библиотеку для выделения плоскостей,
найдите в файле `./ProjectSettings/ProjectSettings.asset` строки
```unityyaml
  scriptingDefineSymbols:
    1: 
```
Добавьте туда значение `NO_MESH_BULDER`, если не собиралась библиотека построения меша,
и значение `NO_PLANES_DETECTION`, если не собиралась библиотека выделения плоскостей.
Пример:
```unityyaml
  scriptingDefineSymbols:
    1: NO_MESH_BULDER;NO_PLANES_DETECTION
```

Собрите проигрыватель через интерфейс редактора Unity.

Перед сборкой нужно будет скомпилировать файлы перевода. Для этого в редакторе Unity выберите меню
`Window->Asset management->Addressables->Gropus` и появившейся вкладке выберите `Build->New build->Default Build Script`.
Дождитесь завершения компиляции и можете собирать проект. Для сборки проекта укажите директорию `./build`,
иначе для плагинов придётся поменять путь к зависимостям на выбранный вами.

### Сборка плагинов

Если вы не собирали библиотеку для получения информации из сети ROS2, добавьте в файл проектка `./plugins/Ros/Ros.csproj` строку
```xml
<DefineConstants>NO_ROS2DDS</DefineConstants>
```
в разделе `<PropertyGroup>`. Пример:
```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <RootNamespace>Elektronik.RosPlugin</RootNamespace>
    <Deterministic>False</Deterministic>
    <AssemblyVersion>1.2.*</AssemblyVersion>
    <FileVersion>1.2.*</FileVersion>
    <DefineConstants>NO_ROS2DDS</DefineConstants>
</PropertyGroup>
```

Для этого опубликуйте (publish) проект. В качестве места публикации выберите
`<PATH_TO_ELEKTRONIK>/build/Plugins/<PLUGIN_NAME>/libraries/`.
```
cd ./plugins
dotnet publish <PLUGIN_NAME> ../build/Plugins/<PLUGIN_NAME>/libraries
```
Затем скопиуйте файл переводов `translations.csv` в директорию `<PATH_TO_ELEKTRONIK>/Plugins/<PLUGIN_NAME>/data`.

Повторите описанные действия для всех плагинов, которые вы хотите установить.

Поздравляем! Электроник собран и готов к работе.

[<- Стартовая страница](Home-RU.md) | [Использование ->](Usage-RU.md)