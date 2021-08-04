# Новое в этой версии:
- Реконструкция поверхностей по облаку точек и информации о наблюдениях
- Теперь построение графа наблюдений происходит на стороне электроника, по этому появилась возможность на ходу фильтровать ребра графа по весу.
- Линукс версия
- Добавлена возможность не компилировать нативные библиотеки, если их функции не нужны.
# Ломающие изменения:
- Обновлённый протокол protobuf требует поменять названия переменных в коде и перекомпилировать.

-----------------------

# New in this version:
- Surface reconstruction based on point cloud and observations info
- Now observations graph is building on Elektronik side and user now can filter graph edges by their weight.
- Linux version
- Now you don't need to build all native libraries if you think you don't need them.
# Breaking changes
- You need to update names in your code and recompile to use updated protobuf protocol.