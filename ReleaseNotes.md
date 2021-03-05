# Новое в этой версии:
- Поддержка плагинов добавляющих новые виды источников данных
- Работа с Protobuf и gRPC вынесена в плагин
- Новое меню настроек
- Переделан UI элемент позволяющий скрывать определённые виды объектов
- Protobuf плагин:
  - Теперь данные загружаются не все сразу, а покадрово
  - Для того чтобы понимать сколько кадров будет в файл может быть записан пакет с метаданными

# New in this version:
- Plugins support. You can now create plugin for your own data source
- Protobuf and gRPC protocols are now in plugin.
- New settings menu
- New GUI element allows you to toggle visibility of cloud objects
- Protobuf plugin:
  - Now file loading frame by frame
  - Metadata can be written in file. Now it is only amount of commands
