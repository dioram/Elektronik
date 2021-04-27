# Новое в этой версии:
- Обновлено окно наблюдений:
    - Изображения теперь перезагружаются с диска при обновлении.
    - Повышено качество картинки.
    - Возможность переключаться между наблюдениями в окне.
    - Экспериментальная возможность просматривать изображения в виде трёхмерной проекции (кликните по наблюдению, когда включен экспериментальный флаг).
- Доработана сетка:
    - Добавлена возможна указать ориентацию сетки, если ваши данные ориентированы не горизонтально. (для этого создайте плоскость с сообщением "Grid")
    - Добавлены отметки координат.
- Добавлена возможность разбивать облака на отдельные сегменты.
    - Алгоритм кластеризации k-means++.
- Исправлены мелкие неудобства при работе с окнами.
- Линейка, позволяет определить расстояние между 2 точками. Может вызывать просадки производительности при большом облаке точек (n>>100 000), поэтому требует включения экспериментального флага.
- Экспериментальные флаги находятся в окне инструментов.

# New in this version:
- Updated observetion window:
    - Images now reloads from disk if they are updated.
    - Image quality improved.
    - Now you can change observation in window.
    - Experimantal function: 3D image. (click on observation when experimental flag is set).
- Updated grid:
    - You can orient grid in case if your cloud isn't oriented horizontally (just create infinite plane with message "Grid").
    - Coordinates.
- Now you can split your cloud on segments.
    - K-means segmentations algorithm.
- Fixed small ui bugs in windows.
- Ruler tool for points. Since it can cause perfomance issues on big point cloud (n >> 100 000) you should enable experimantal flag first.
- You can find experimental flags in tools window
