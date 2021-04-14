# Окна для вывода дополнительных данных.

Электроник имеет систему окон, которые позволяют выводить пользователю любые данные,
не ограничиваясь лишь облаками объектов. Например, текстовые данные или табличные.

## Виды окон

### Окно вывода текста

![Текстовое окно](Images/TextWindow.png)
- Имеет 2 режима работы:
  - Вывод 1 сообщения (Следующее сообщение заменит предыдущее)
  - Вывод списка сообщений (из-за особенностей работы unity с текстом выводятся только последние 10 сообщений)
  
### Окно вывода таблиц

![Окно таблиц](Images/TableWindow.png)
*Из-за особенностей работы unity с текстом добавление очередной строки в таблицу - медленная операция, 
по этому по умолчанию выводятся последние 10 строк таблицы. Либо можно загрузить все данные сразу.*

### Окно вывода изображений

![Окно изображений](Images/ImageWindow.png)

### Окно вывода информации о наблюдении (изображение + текст)

![ObservationWindow.png](Images/ObservationWindow.png)

### Окно вывода специальной информации (protobuf)

![SpecialInfoWindow.png](Images/SpecialInfoWindow.png)

### Окно настроек

![SettingsWindow.png](Images/SettingsWindow.png)

[<- Написать свой плагин](Plugins-RU.md) | [Protobuf plugin ->](Protobuf-RU.md)