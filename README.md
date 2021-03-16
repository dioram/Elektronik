# Elektronik Tools
The tool for testing and debugging SLAM.

![](https://user-images.githubusercontent.com/29245436/61538805-da631200-aa42-11e9-8431-44feb81fdbb3.png)

[![Build Status](https://elevir.visualstudio.com/Elektronik%20tool%202.0%20pipelines/_apis/build/status/Elektronik%20tool%202.0%20pipelines-CI?branchName=master)](https://elevir.visualstudio.com/Elektronik%20tool%202.0%20pipelines/_build/latest?definitionId=4&branchName=master)

Elektronik Tools is a tool that is actively used by the Dioram development team in developing tracking algorithms and SLAM. This software allows you to visually track changes in the point cloud and the connectivity graph of observations in a map constructed using the SLAM algorithm; also, this program allows you to observe the tracks of motion of tracked objects (for example, the track of a VR helmet and the reference track). This greatly simplifies the process of debugging the map construction mode, relocalization mode, and many other things related to tracking algorithms and SLAM.

There are two main modes available in Elektronik - realtime mode and reading from a file mode.

In the realtime mode you can observe the process of building a map whilst the algorithm is running by transmitting data in binary format via TCP.
The offline mode works by downloading data from a file. It allows you to write all events once and run them without running the main algorithm. This mode supports greater displaying opportunities than online mode, because the offline mode has lower performance requirements. This mode provides you such features as rewinding events, viewing information about points and observations, “playing” events, etc. You can see the detailed overview of all features in the corresponding Wiki section.

In addition to the main modes, there is also an additional VR mode. This mode allows you to follow all process from a VR helmet, for example, you can walk inside the point cloud built. In the VR mode, it is possible to disable tracking of the helmet, connected to your PC. This allows you to use your own tracking to move around the scene. For example, you can connect a Microsoft Mixed Reality helmet but use your own tracking instead of that provided via the installed helmet driver.

If you want to add, improve or accelerate Elektronik, you can find all necessary information in Wiki section along with a description of the source code structure. We try to make the Elektronik code as convenient as possible for debugging and supporting, so we actively use OOP techniques and patterns.

We hope that Elektronik will help you in developing the SLAM of your dream and will be grateful for your help in its development!

Sincerely, Dioram development team.
