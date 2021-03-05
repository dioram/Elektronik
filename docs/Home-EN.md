## Elektronik Tools 2.0 

is a tool that is actively used by the Dioram development team in developing tracking algorithms and SLAM.
This software allows you to visually track changes in the point cloud and the connectivity graph of observations in a map
constructed using the SLAM algorithm; also, this program allows you to observe the tracks of motion of the tracked objects
(for example, the track of the VR helmet and the reference track).
This greatly simplifies the process of debugging the map construction mode, relocalization mode,
and many other things related to tracking algorithms and SLAM.

This documentation will help you to get familiar with the interface of the application and to understand the API.

Choose a section:
- [Usage](Usage-EN.md)
- [Data transfer format](Data-EN.md)
- [Internal API](API-EN.md)

### Update from 2.0 on 3.0 and above

Since version 3.0 Elektronik uses [google protocol buffers](https://developers.google.com/protocol-buffers/?hl=en) 
and [gRPC](https://grpc.io/).