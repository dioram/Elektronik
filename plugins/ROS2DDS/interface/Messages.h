#ifndef ROS2DDS_MESSAGES_H
#define ROS2DDS_MESSAGES_H

#include "../DataTypes/Ros.h"

class Ros2Message
{
public:
    Ros2Message(const std::string &topic, const std::string &type) : Topic(topic), Type(type)
    {}

    std::string Topic;
    std::string Type;

    static Ros2Message *Parse(void *sample, const std::string &topic, const std::string &type);
};

class OdometryMessage : public Ros2Message
{
public:
    OdometryMessage(nav_msgs::msg::Odometry *sample, const std::string &topic, const std::string &type)
            : Ros2Message(topic, type),
              pos_x(sample->pose().pose().position().x()),
              pos_y(sample->pose().pose().position().y()),
              pos_z(sample->pose().pose().position().z()),
              rot_x(sample->pose().pose().orientation().x()),
              rot_y(sample->pose().pose().orientation().y()),
              rot_z(sample->pose().pose().orientation().z()),
              rot_w(sample->pose().pose().orientation().w())
    {}

    double pos_x;
    double pos_y;
    double pos_z;
    double rot_x;
    double rot_y;
    double rot_z;
    double rot_w;
};

class PoseStampedMessage : public Ros2Message
{
public:
    PoseStampedMessage(geometry_msgs::msg::PoseStamped *sample, const std::string &topic, const std::string &type)
            : Ros2Message(topic, type),
              pos_x(sample->pose().position().x()),
              pos_y(sample->pose().position().y()),
              pos_z(sample->pose().position().z()),
              rot_x(sample->pose().orientation().x()),
              rot_y(sample->pose().orientation().y()),
              rot_z(sample->pose().orientation().z()),
              rot_w(sample->pose().orientation().w())
    {

    }

    double pos_x;
    double pos_y;
    double pos_z;
    double rot_x;
    double rot_y;
    double rot_z;
    double rot_w;
};

class PointFieldM
{
public:
    PointFieldM(const std::string& name, uint32_t offset) : Name(name), Offset(offset)
    {}

    std::string Name;
    uint32_t Offset;
};

class PointCloud2Message : public Ros2Message
{
public:
    PointCloud2Message(sensor_msgs::msg::PointCloud2 *sample, const std::string &topic, const std::string &type)
            : Ros2Message(topic, type),
            height(sample->height()),
            width(sample->width()),
            point_step(sample->point_step()),
            row_step(sample->row_step()),
            data(sample->data())
    {
        for (auto field: sample->fields())
        {
            fields.emplace_back(field.name(), field.offset());
        }
    }

    uint32_t height;
    uint32_t width;
    std::vector<PointFieldM> fields;
    uint32_t point_step;
    uint32_t row_step;
    std::vector<uint8_t> data;
};

inline Ros2Message *Ros2Message::Parse(void *sample, const std::string &topic, const std::string &type)
{
    if (type == "nav_msgs::msg::dds_::Odometry_") {
        return new OdometryMessage((nav_msgs::msg::Odometry *) sample, topic, type);
    }
    if (type == "geometry_msgs::msg::dds_::PoseStamped_") {
        return new PoseStampedMessage((geometry_msgs::msg::PoseStamped *) sample, topic, type);
    }
    if (type == "sensor_msgs::msg::dds_::PointCloud2_") {
        return new PointCloud2Message((sensor_msgs::msg::PointCloud2 *) sample, topic, type);
    }
    return new Ros2Message(topic, type);
}

#endif //ROS2DDS_MESSAGES_H
