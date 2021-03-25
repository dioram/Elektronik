#ifndef ROS2DDS_LOGGER_H
#define ROS2DDS_LOGGER_H


class Logger
{
public:
    virtual ~Logger() = default;

    virtual void Info(const std::string &message)
    {
        std::cout << message << std::endl;
    }

    virtual void Error(const std::string &message)
    {
        std::cerr << message << std::endl;
    }
};


#endif //ROS2DDS_LOGGER_H
