# Windows for additional data

Elektronik has built-in windows system. That allows to show user any kind of data,
not only clouds of objects. For example it can be text data or tables or images.

## Types of windows

### Text window

![Text window](Images/TextWindow.png)
- Has 2 modes:
  - 1 message (Next message will replace previous)
  - List of messages (Next messages will be added to the end) 
    *Due to limitation of Unity text renderer it can only show 10 last messages*
    
### Table window

![Окно таблиц](Images/TableWindow.png)
*Due to limitation of Unity text renderer adding of new row in table is very slow operation.
So, this type of windows shows only 10 last rows of table. Or you can preload all data.*

### Image window

![Image window](Images/ImageWindow.png)

### Observation window (image + text)

![ObservationWindow.png](Images/ObservationWindow.png)

### Special info window (protobuf)

![SpecialInfoWindow.png](Images/SpecialInfoWindow.png)

[<- Write your own plugin](Plugins-EN.md) | [Protobuf plugin ->](Protobuf-EN.md)