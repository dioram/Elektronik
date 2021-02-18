# Introduction

Elektronik uses [google protocol buffers](https://developers.google.com/protocol-buffers/?hl=en)
and [gRPC](https://grpc.io/).
All Elektronik data is represented as data packages. 
You can find package specification in protobuf format in file [MapTypes.proto](../Assets/Scripts/Common/Data/Protobuf/MapTypes.proto).

There are several types of packages:
1. Action package
2. Image package

# Action package

Each such package contains:
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
   * Infinite planes

## Data actions

There are four actions over data:
1. Add. This action adds neccesary objects to a map. 
   Note, that without objects adding, you can't interact with them in a future. 
   If you try to apply anything to unexisted object, then an error will raise in Elektronik.
2. Update. This action changes object parameters according to received data.
3. Remove. Removes an object from a map.
4. Clear. Clear a map corresponding an object type.
5. Info. Shows additional information in offline mode.

## Timestamp

Timestamp is used by the Elektronik offline mode, it is displayed on an actions rewinding slider 
in the right-bottom corner of a window. 
Also it will be used to find image form camera for this moment of time.
Image file should be called \<timestamp\>.png.

Online mode doesn't use this information.

## Special package flag

Special package flag is used by offline mode, it is intended 
to skip non interesting actions by clicking rewinding buttons to jump to nearest special package.

## Message

A message is displayed for all special packages in the right-top corner of window in offline mode.

## Connections set

A connections set is intended to display lines between objects such as points or observations. 
A connection is a tuple of two integer values which contain ids of connecting objects. 
At the moment it is possible to setup connections just between same type objects,
e.g. there is no possibility to connect a point with an observation. 
Note, that a connection will be updated automatically while object updating.

Besides connections, a set also contains an action. It may be one of two following actions:
* Add
* Remove

In both cases an action can be done just over existing objects in a map, 
but connections have an effect just while objects updating. For example, to add (remove) connections between two points, 
you should send a package with an "update" action for points and an "add" ("remove") action for connections. 
Also connections can be added and without actual update for objects, 
e.g. you can create a package which will contain an empty set of objects and non empty set of connections.

## Data

There are several types of data:
1. Points
2. Observations
3. Lines
4. Tracked objects
5. Infinite planes

Any package must have one set of objects of type above.

### Points

Point object contains:
1. Point ID.
2. Position
3. Color
4. Message

### Observations

Observation object contains:
1. Point
2. Orientation (quaternion)
3. Message
4. File name of image taken at this observation
5. Statistics (not using for now)

### Lines

Lines object contains:
1. First connected point
2. Second connected point

### Tracked objects

Tracked objects are any object that has position, orientation and also trajectory.
It can be VR-helmet or any other device that taking SLAM-data.

Tracked objects contains:
1. Object ID
2. Poistion
3. Orientation (quaternion)
4. Trajectory color
5. Message

### Infinite planes

Infinite plane object contains:
1. Plane ID
2. Point on plane
3. Normal
4. Color
5. Message

# Image package

This type of packages is used for receive images form camera in online mode.
It contains byte array of image in PNG or JPEG format.

# C#

You can familiar with Elektronik interaction example in the folder Examples/csharp at the root of repo. 
The example is implemented as Unit-tests. Before starting tests you should run Elektronik in online mode. 
After tests was ran, there will be files \<TestName\>.dat in the executable files directory folder 
which can be used as Elektronik opportunities presentation in offline mode.

# C++

You can familiar with Elektronik interaction example in the folder Examples/cxx at the root of repo. 
The example is implemented as google tests Unit-tests. To build this example we recommend to use 
[vcpkg](https://github.com/Microsoft/vcpkg).

[<- Usage](Usage-EN.md) | [Internal API ->](API-EN.md)