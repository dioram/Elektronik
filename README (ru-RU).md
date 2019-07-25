# Elektronik-Tools-2.0

![](https://user-images.githubusercontent.com/29245436/61538805-da631200-aa42-11e9-8431-44feb81fdbb3.png)

[![Build Status](https://elevir.visualstudio.com/Elektronik%20tool%202.0%20pipelines/_apis/build/status/Elektronik%20tool%202.0%20pipelines-CI?branchName=version_2_0_2)](https://elevir.visualstudio.com/Elektronik%20tool%202.0%20pipelines/_build/latest?definitionId=4&branchName=version_2_0_2)

### English language

The tool for testing and debugging SLAM.

Elektronik Tools 2.0 is a tool that is actively used by the Dioram development team in developing tracking algorithms and SLAM. This software allows you to visually track changes in the point cloud and the connectivity graph of observations in a map constructed using the SLAM algorithm; also, this program allows you to observe the tracks of motion of tracked objects (for example, the track of a VR helmet and the reference track). This greatly simplifies the process of debugging the map construction mode, relocalization mode, and many other things related to tracking algorithms and SLAM.

There are two main modes available in Elektronik - realtime mode and reading from a file mode.

In the realtime mode you can observe the process of building a map whilst the algorithm is running by transmitting data in binary format via TCP.
The offline mode works by downloading data from a file. It allows you to write all events once and run them without running the main algorithm. This mode supports greater displaying opportunities than online mode, because the offline mode has lower performance requirements. This mode provides you such features as rewinding events, viewing information about points and observations, “playing” events, etc. You can see the detailed overview of all features in the corresponding Wiki section.

In addition to the main modes, there is also an additional VR mode. This mode allows you to follow all process from a VR helmet, for example, you can walk inside the point cloud built. In the VR mode, it is possible to disable tracking of the helmet, connected to your PC. This allows you to use your own tracking to move around the scene. For example, you can connect a Microsoft Mixed Reality helmet but use your own tracking instead of that provided via the installed helmet driver.

We also understand that the Unity engine is not the fastest of the existing engines, so we are certainly paying attention to performance. Elekronik uses object pools and points that are represented by meshes. This allows to display large point clouds with high performance and without lags.

If you want to add, improve or accelerate Elektronik, you can find all necessary information in Wiki section along with a description of the source code structure. We try to make the Elektronik code as convenient as possible for debugging and supporting, so we actively use OOP techniques and patterns.

We hope that Elektronik will help you in developing the SLAM of your dream and will be grateful for your help in its development!

Sincerely, Dioram development team.

### Руский язык

Инструмент для тестирования и отладки SLAM.

Elektronik Tools 2.0 - это инструмент на базе движка Unity, который активно используется командой разработчиков Dioram при разработке алгоритмов трекинга и  SLAM. Данное программное обеспечение позволяет визуально отслеживать изменения облака точек и графа связности наблюдений в карте, построенной при помощи алгоритма SLAM; также эта программа позволяет наблюдать траектории движения отслеживаемых объектов (например, траекторию VR шлема и эталонную траекторию). Это значительно упрощает процесс отладки режима построения карты, режима релокализации и многого другого связанного с алгоритмами трекинга и SLAM.

В электронике доступны два основных режима - режим реального времени и режим чтения из файла. 

В режиме реального времени Вы сможете наблюдать за процессом построения карты во время работы алгоритма, просто передавая данные в двоичном формате по протоколу TCP.

Оффлайн режим работает через загрузку данных из файла, это позволит Вам один раз записать все события в файл и прогонять эти события без запуска основного алгоритма. Этот режим поддерживает большие возможности по отображению информации, чем онлайн режим, так как к режиму оффлайн предъявляются меньшие требования по производительности. Данный режим предоставляет такие возможности как перемотка событий, просмотр информации о точках и наблюдениях, "проигрывание" событий и пр. Подробный обзор всех возможностей Вы можете просмотреть в соответствующем разделе Wiki.

Помимо основных режимов также присутствует дополнительный VR режим. Этот режим позволит Вам следить за всеми процессами из шлема виртуальной реальности, например, Вы сможете пройтись внутри построенного облака точек! В VR режиме есть возможность отключать трекинг шлема, который Вы подключили к компьютеру. Это позволит Вам использовать собственный трекинг для перемещения по сцене. Например, Вы можете подключить шлем Microsoft Mixed Reality, но трекинг осуществлять свой, а не предоставленный через драйвер установленного шлема.

Мы также понимаем, что игровой движок Unity не самый быстрый из существующих движков, поэтому безусловно уделяем внимание производительности. В Elektronik используются пулы объектов, а точки представлены через меши (Mesh), данный аспект позволяет отображать большие облака точек с высокой производительностью и без тормозов (по-крайней мере на нашем оборудовании LOL).

Если у Вас появится желание дополнить, поправить или ускорить Электроника, то всю необходимую информацию Вы сможете найти в разделе Wiki посвящённому описанию структуры исходного кода Электроника. Мы стараемся делать код Электроника как можно удобнее для отладки и сопровождения, поэтому в коде активно используются приёмы и паттерны ООП.

Надеемся, что Электроник поможет Вам в разработке SLAM Вашей мечты и будем признательны за Вашу помощь в его развитии!

С уважением, команда разработчиков Dioram.
