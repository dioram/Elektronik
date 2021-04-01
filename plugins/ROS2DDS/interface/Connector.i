%module(directors = "1") ROS2DDS
%{
#include "interface/Connector.h"
#include "interface/Handlers.h"
#include "interface/Messages.h"
#include "interface/Logger.h"
%}
%include "stdint.i"
%include "std_string.i"
%include "std_vector.i"
%feature("director") MessageHandler;
%feature("director") TopicsDiscoveryHandler;
%feature("director") Ros2Message;
%feature("director") OdometryMessage;
%feature("director") ImageMessage;
%feature("director") PoseStampedMessage;
%feature("director") PointCloud2Message;
%feature("director") Logger;
namespace std {
   %template(vectorp) vector<PointFieldM>;
   %template(vectoru8) vector<uint8_t>;
};
%include "Connector.h"
%include "Handlers.h"
%include "Messages.h"
%include "Logger.h"