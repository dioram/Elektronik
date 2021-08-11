## {Название}
### Основные изменения:
- Изменение 1
- Изменение 2
### Ломающие изменения (если есть):
- Изменение 1

------------------------------
### Чек-лист тестирования перед релизом:
- Сборка через CI выполняется
  - [ ] Windows
  - [ ] Linux
- Авто-тесты пройдены
  - [ ] Windows
  - [ ] Linux
- Ручные тесты protobuf-online (`Protobuf.Tests.Elektronik`) пройдены (кроме `ComplexStressTest`, его не надо запускать)
  - [ ] Windows
  - [ ] Linux
- Протестировать protobuf-offline (`./Examples/a_lot_of_points.dat`). Обратить внимание на: старт/паузу, остановку, переметку вперёд и назад.
  - [ ] Windows
  - [ ] Linux
- Протестировать работу ROS1 примере [этого](https://open-source-webviz-ui.s3.amazonaws.com/demo.bag) пакета.
  - Онлайн
    - [ ] Windows
    - [ ] Linux
  - rosbag
    - [ ] Windows
    - [ ] Linux
- Протестировать онлайн ROS2 на пакете из примера ROS1.
  - [ ] Windows
  - [ ] Linux
- Протестировать rosbag2 на пакете `./Plugins/Ros.Tests/test_db.db3`
  - [ ] Windows
  - [ ] Linux
- Протестировать кластеризацию
  - K-means
    - [ ] Windows
    - [ ] Linux
  - Planes detection
    - [ ] Windows
    - [ ] Linux
- Протестировать снятие, сохранение и загрузку снапшотов
  - [ ] Windows
  - [ ] Linux
- Протестировать запись последовательности и её воспроизведение
  - [ ] Windows
  - [ ] Linux
- Протестировать окна: закрытие/открытие, сворачивание/разворачивание, изменение размеров, прилипание
  - [ ] Windows
  - [ ] Linux
- Проверить окно наблюдений, оно должно появляться при наведении на наблюдение, закрепляться при нажатии, 
позволять перемещаться между наблюдениями, позволять навести камеру на наблюдение, отображать изображения.
  - [ ] Windows
  - [ ] Linux
- Проверить работу линейки: выбор двух точек, отмена выбора по esc/mmb, удаление измерения
  - [ ] Windows
  - [ ] Linux
- Проверить, что если включить отображение следа за точкой, то оно работает корректно.
На примере `./Examples/a_lot_of_points.dat` и Protobuf-онлайн.
  - [ ] Windows
  - [ ] Linux
- Проверить, что на `./Examples/a_lot_of_points.dat` корректно работает отображение и фильтрация связей.
  - [ ] Windows
  - [ ] Linux
- Проставить везде корректную версию релиза
  - [ ] В настройках проекта
  - [ ] В `package/DEBIAN/control`
  - [ ] Создать в репозитории тег с нужной версией
- [ ] Обновить ReleaseNotes