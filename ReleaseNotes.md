![GitHub release (by tag)](https://img.shields.io/github/downloads/dioram/Elektronik-Tools-2.0/v3.2.0-rc6/total)

# Новое в этой версии:
- Реконструкция поверхностей по облаку точек
- Теперь построение графа наблюдений происходит на стороне электроника, по этому появилась возможность на ходу фильтровать ребра графа по весу.
- Линукс версия
- Добавлена возможность не компилировать нативные библиотеки, если их функции не нужны.
- Добавлена возможность менять масштаб сцены "на лету".
- Добавлена возможность менять скорость воспроизведения в оффлайн режиме. 
# Ломающие изменения:
- Обновлённый протокол protobuf требует поменять названия переменных в коде и перекомпилировать.

-----------------------

# New in this version:
- Surface reconstruction based on point cloud
- Now observations graph is building on Elektronik side and user now can filter graph edges by their weight.
- Linux version
- Now you don't need to build all native libraries if you think you don't need them.
- Now you can change scene scale on the fly.
- Now you can change play speed in offline mode.
# Breaking changes
- You need to update names in your code and recompile to use updated protobuf protocol.