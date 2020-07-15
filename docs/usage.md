# Introduction

Starting with version 3.0 the Elektronik uses [google protocol buffers](https://developers.google.com/protocol-buffers/?hl=en) data exchange format and [gRPC](https://grpc.io/). You can familiar with documentation for these frameworks by presented references.

If you've used the version 2 of Elektronik before, then you must remove the folder:

C:\\Users\\<user_name>\\AppData\\LocalLow\\Dioram\\Elektronik tools 2_0

That folder contains a history of recently opened files and recent connections addresses. A storage format of such information has been changed and old format will lead to errors. A new storage format is a json, and, if you need, it allows to add quick access addresses manually.

All Elektronik data is represented as data packages. Each package contains:
1. Data action:
   * add
   * update
   * remove
   * clear
   * info
2. Timestamp
3. Special package flag (special)
4. Message
5. Connections set
6. Data:
   * Points
   * Observations
   * Lines
   * Tracked objects

# Data actions

There are four actions over data:
1. Add. This action adds neccesary objects to a map. Note, that without objects adding, you can't interact with them in a future. If you try to apply anything to unexisted object, then an error will raise in Elektronik.
2. Update. This action changes object parameters according to received data.
3. Remove. Removes an object from a map.
4. Clear. Clear a map corresponding an object type.

## Timestamp

Timestamp is used by the Elektronik offline mode, it is displayed on an actions rewinding slider in the right-bottom corner of a window. Online mode doesn't display this information.

## Special package flag

Special package flag is used by offline mode, it is intended to skip non interesting actions by clicking rewinding buttons to jump to nearest special package.

## Message

A message is displayed for all special packages in the right-top corner of window in offline mode.

## Connections set

A connections set is intended to display lines between objects such as points or observations. A connection is a tuple of two integer values which contain ids of connecting objects. At the moment it is possible to setup connections just between same type objects, e.g. there is no possibility to connect a point with an observation. Note, that a connection will be updated automatically while object updating.

Besides connections, a set also contains an action. It may be one of two following actions:
  * Add
  * Remove

In both cases an action can be done just over existing objects in a map, but connections have an effect just while objects updating. For example, to add (remove) connections between two points, you should send a package with an "update" action for points and an "add" ("remove") action for connections. Also connections can be added and without actual update for objects, e.g. you can create a package which will contain an empty set of objects and non empty set of connections.

## Data

At the moment there are four types of data:
1. Points
2. Observations
3. Lines
4. Tracked objects

Any package must have one set of objects of type above.

# C#

You can familiar with Elektronik interaction example in the folder Examples/csharp at the root of repo. The example is implemented as Unit-tests. Before starting tests you should run Elektronik in online mode. After tests was ran, the file PointsStressTests.dat appears in the executable files directory folder which can be used as Elektronik opportunities presentation in offline mode.

# C++

You can familiar with Elektronik interaction example in the folder Examples/cxx at the root of repo. The example is implemented as google tests Unit-tests. To build this example we recommend to use [vcpkg](https://github.com/Microsoft/vcpkg).