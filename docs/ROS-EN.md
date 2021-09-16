# Plugin for rendering data from ROS network

This plugins allows Elektronk to render data transmitted via [ROS](https://www.ros.org/) protocol.

## Supported message types

At this moment Elektronik supports only these types of ROS messages:
- *sensor_msgs/PointCloud2*, as point cloud;
- *geometry_msgs/PoseStamped*, as tracked objects;
- *nav_msgs/Odometry*, as tracked objects;
- *visualization_msgs/MarkerArray*, as point and line clouds, but only for reading form rosbag2 file.

## Modes

This plugin has 4 modes:
1) Direct reading from rosbag.
    - Select offline mode and **"ROS bag"** plugin, and choose path to "*.bag" file.
      For now plugin supports only uncompressed bags.
2) Direct reading from rosbag2.
    - Select offline mode and **"ROS bag"** plugin, and choose path to "*.db3" or metadata.yaml file.
3) Rendering data transmitted via ROS.
    - For that you have to use [Rosbridge](http://wiki.ros.org/rosbridge_suite/Tutorials/RunningRosbridge).
      In first terminal launch **roscore**, and in this second one - rosbridge. 
      After rosbridge is launched you will see address and port of bridge, use them to connect Elektronik.
      Launch Elektronik and select online mode and **"ROS listener"** plugin.
      Elektronik will find all existed topics and subscribe on ones with supported type.
      On discovery of new topics Elektronic will also subscribe on them.
4) ОRendering data transmitted via ROS2.
    - Select online mode and **"ROS2 listener"** plugin. Set ROS domain ID. If you don't know domain id of you ROS network,
      then most likely it used default value - 0.
      Due to ROS2 protocol limitation Elektronik can't discover existed topics and will only subscribe on 
      topics launched after Elektronic started.

[<- Protobuf](Protobuf-EN.md) | [Return to home page](Home-EN.md)