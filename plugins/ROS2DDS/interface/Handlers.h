#ifndef ROS2DDS_HANDLERS_H
#define ROS2DDS_HANDLERS_H

#include "Messages.h"

class MessageHandler {
public:
    virtual ~MessageHandler() = default;
    virtual void Handle(Ros2Message* message) {}
};

class TopicsDiscoveryHandler {
public:
    virtual ~TopicsDiscoveryHandler() = default;
    virtual void Handle(const std::string& name, const std::string& type) {}
};

#endif //ROS2DDS_HANDLERS_H
