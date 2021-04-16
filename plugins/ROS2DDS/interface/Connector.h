#ifndef CONNECTOR_H
#define CONNECTOR_H

#include <algorithm>
#include <iostream>
#include <map>
#include <string>
#include <fastdds/dds/subscriber/DataReader.hpp>
#include <fastdds/dds/topic//TopicDescription.hpp>
#include <fastdds/dds/subscriber/SampleInfo.hpp>
#include <fastdds/dds/domain/DomainParticipant.hpp>
#include <fastdds/dds/domain/DomainParticipantListener.hpp>

#include "Handlers.h"
#include "Logger.h"

using namespace std;

class Connector
{
public:
    explicit Connector(int domainId, Logger *logger);

    virtual ~Connector();

    void SubscribeOnNewTopics(TopicsDiscoveryHandler *handler);

    void UnsubscribeFromNewTopics(TopicsDiscoveryHandler *handler);

    void SubscribeOnMessages(const std::string& topicName, const std::string& topicType, MessageHandler *handler);

    void UnsubscribeFromMessages(const string &name, MessageHandler *handler);

private:
    eprosima::fastdds::dds::DomainParticipant *m_topicsCollectorParticipant;
    eprosima::fastdds::dds::DomainParticipant *m_topicsReaderParticipant;
    eprosima::fastdds::dds::Subscriber* m_subscriber;
    map<string, vector<MessageHandler *>> m_handlers;
    map<eprosima::fastdds::dds::DataReader *, std::pair<std::string, std::string>> m_readers;
    vector<TopicsDiscoveryHandler *> m_topicsHandlers;
    map<string, eprosima::fastdds::dds::TypeSupport> m_supportedTypes;
    vector<eprosima::fastdds::dds::Topic *> m_topics;
    Logger *m_logger;

    class PubListener : public eprosima::fastdds::dds::DomainParticipantListener
    {
    public:
        explicit PubListener(Connector *connector) : m_connector(connector)
        {}

        void on_publisher_discovery(eprosima::fastdds::dds::DomainParticipant *participant,
                                    eprosima::fastrtps::rtps::WriterDiscoveryInfo &&info) override;

    private:
        Connector *m_connector;
    } m_pubListener;

    friend PubListener;

    class MessageListener : public eprosima::fastdds::dds::SubscriberListener
    {
    public:
        explicit MessageListener(Connector *connector) : m_connector(connector)
        {}

        void on_data_available(eprosima::fastdds::dds::DataReader *reader) override;

    private:
        Connector *m_connector;
    } m_messageListener;

    friend MessageListener;

    void CreateReader(const string &topicName, const string &topicType);
};

#endif // CONNECTOR_H