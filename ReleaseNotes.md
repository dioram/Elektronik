![GitHub release (by tag)](https://img.shields.io/github/downloads/dioram/Elektronik/v4.0.0-rc2/total)

# Новое в этой версии
- Новый тип отображаемых объектов - маркеры. 
  - Они представляют собой некий примитив из списка с закреплённым текстом.
  - Примитивы: куб, полупрозрачный куб, сфера, полупрозрачная сфера, кристал.
  - Примитивам можно установить позицию, ориентацию, цвет и масштаб по трём осям.
- Наблюдения:
  - Наблюдения теперь не изменяются в размерах при масштабировании сцены.
  - Размер наблюдений можно менять поллзунком в настройках сцены.
  - Рендер наблюдений теперь вынесен на GPU ещё большей частью.
  - Исправлен баг, когда криво считывалось наведение курсора на наблюдение, и нажатие на него.
- Построение меша:
  - Улучшен алгоритм построения меша для плотных облаков.
  - Меш теперь может быть отрендерен в качестве полигональной сетки без заливки.
- Экран загрузки.
- Рефакторинг рендера.
- Исправлен баг с бесконечной загрузкой, если окно "Дерево данных" было закрыто или свёрнуто.
- Добавлена возможность выбирать проекцию камеры: перспективную или ортогональную.
- Плагины:
  - Protobuf:
    - Изображения с камеры теперь могут быть переданы как путь к картинкам.
  - Planes detection:
    - Теперь точки могут принадлежать нескольким плоскостям одновременно.
    
-----------------------

# New in this version
- New type of objects - Visualisation markers.
  - It is some primitive from list with text.
  - Primitives: cube, semitransparent cube, sphere, semitransparent sphere, crystal.
  - Primitives have position, orientation, color and scale.
- Observations:
  - Observations now don't change their size when scene is scaling.
  - Observations size now can be set by slider in scene settings.
  - Observations render now moved on GPU a little more.
  - Fixed bug where hovering and clicking on observation handles poorly.
- Meshes:
  - Mesh reconstruction algorithm now works better with dense point clouds.
  - Meshes now can be rendered as wireframe.
- Loading screen was added.
- Refactoring of render pipeline.
- Fixed bug where Elektronik stuck on loading screen if "Data tree" window was closed or minimized.
- Now user can choose camera projection: perspective or orthographic.
- Plugins
  - Protobuf:
    - Images from camera now can be sent as paths to images.
  - Planes detection:
    - Now one point can belong to several planes.