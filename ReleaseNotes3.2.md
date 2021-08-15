![GitHub release (by tag)](https://img.shields.io/github/downloads/dioram/Elektronik-Tools-2.0/v3.2.0/total)

# Новое в этой версии:
- Начиная с этой версии возможно автоматическое обновление Электроника при выходе нового релиза или пре-релиза.
- Окно настроек сцены преобразовано в 2 окна: окно инструментов и окно дерева источников данных.
- Добавлен виджет для вывода ориентации и позиции камеры, а также для выравнивания камеры относительно осей координат.
- Наблюдения и отслеживаемые объекты теперь могут отображать цвет.
- Обновлено окно наблюдений:
  - Изображения теперь перезагружаются с диска при обновлении.
  - Повышено качество картинки.
  - Возможность переключаться между наблюдениями в окне.
- Доработана сетка:
  - Добавлена возможна указать ориентацию сетки, если ваши данные ориентированы не горизонтально. (для этого создайте плоскость с сообщением "Grid")
  - Добавлены отметки координат.
- Добавлена возможность разбивать облака на отдельные сегменты.
  - Алгоритм кластеризации k-means++.
  - По плоскостям.
- Линейка позволяющая определить расстояние между 2 точками.
- Снимки состояний облака:
  - Теперь вы можете сделать снимок текущего состояния облака, чтобы потом оценить изменения облака.
  - Снимки можно сохранять на диск и загружать от туда.
- Запись последовательностей.
- Возможность выключить/включить логирование в файл.
- Возможность передавать данные на другие запущенные экземпляры приложения.
- Возможность менять прозрачность линий.
- Реконструкция поверхностей по облаку точек и информации о наблюдениях.
- Теперь построение графа наблюдений происходит на стороне электроника, по этому появилась возможность на ходу фильтровать ребра графа по весу.
- Линукс версия
- Добавлена возможность не компилировать нативные библиотеки, если их функции не нужны.
- Добавлена возможность менять масштаб сцены "на лету".
- Добавлена возможность менять скорость воспроизведения в оффлайн режиме.
# Ломающие изменения:
- Обновлённый протокол protobuf требует поменять названия переменных в коде и перекомпилировать.

-----------------------

# New in this version:
- Starts with this version Elektronik can be automatically updated at new release or pre-release.
- Scene settings window was separated to tools window and data source tree window.
- Added widget showing camera position, orientation. This widget can also align camera.
- Observations and tracked objects now can render color.
- Updated observetion window:
  - Images now reloads from disk if they are updated.
  - Image quality improved.
  - Now you can change observation in window.
- Updated grid:
  - You can orient grid in case if your cloud isn't oriented horizontally (just create infinite plane with message "Grid").
  - Coordinates.
- Now you can split your cloud on segments.
  - K-means segmentations algorithm.
  - Segmentation by planes.
- Ruler tool for points.
- Snapshots
  - Now you can create snapshot of all clouds.
  - Snapshots can be saved on disk and loaded from there.
- Recording of incoming data
- User can disable/enable logging in file.
- Plugin that allows Elektronik to transmit data from one instance to another.
- Semitransparent lines.
- Camera can follow tracked objects.
- Surface reconstruction based on point cloud and observations info.
- Now observations graph is building on Elektronik side and user now can filter graph edges by their weight.
- Linux version
- Now you don't need to build all native libraries if you think you don't need them.
- Now you can change scene scale on the fly.
- Now you can change play speed in offline mode.
# Breaking changes
- You need to update names in your code and recompile to use updated protobuf protocol.