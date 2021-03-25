#include "Connector.h"

#include <fastdds/dds/subscriber/qos/DataReaderQos.hpp>
#include <fastdds/dds/domain/DomainParticipantFactory.hpp>
#include <fastdds/dds/subscriber/Subscriber.hpp>

#include "../DataTypes/RosPubSubTypes.h"
#include "Logger.h"

using namespace eprosima::fastdds::dds;
using namespace sensor_msgs::msg;
using namespace geometry_msgs::msg;
using namespace nav_msgs::msg;

Connector::Connector(int domainId, Logger *logger) : m_messageListener(this),
                                                     m_pubListener(this),
                                                     m_logger(logger)
{
    DomainParticipantQos pqos;
    pqos.name("Elektronik_topics_collector");
    m_topicsCollectorParticipant = DomainParticipantFactory::get_instance()->create_participant(domainId, pqos,
                                                                                                &m_pubListener);
    DomainParticipantQos pqos1;
    pqos1.name("Elektronik_topics_reader");
    pqos1.transport().listen_socket_buffer_size = 12582912;
    m_topicsReaderParticipant = DomainParticipantFactory::get_instance()->create_participant(domainId, pqos1);
    m_subscriber = m_topicsReaderParticipant->create_subscriber(SUBSCRIBER_QOS_DEFAULT, nullptr);

    m_supportedTypes["nav_msgs::msg::dds_::Odometry_"] = TypeSupport(new OdometryPubSubType());
    m_supportedTypes["geometry_msgs::msg::dds_::PoseStamped_"] = TypeSupport(new PoseStampedPubSubType());
    m_supportedTypes["sensor_msgs::msg::dds_::PointCloud2_"] = TypeSupport(new PointCloud2PubSubType());

    for (const auto &pair: m_supportedTypes) {
        pair.second.register_type(m_topicsReaderParticipant);
    }
}

Connector::~Connector()
{
    for (auto topic: m_topics) {
        m_topicsReaderParticipant->delete_topic(topic);
    }
    for (const auto &pair: m_readers) {
        m_subscriber->delete_datareader(pair.first);
    }
    m_topicsReaderParticipant->delete_subscriber(m_subscriber);
    DomainParticipantFactory::get_instance()->delete_participant(m_topicsCollectorParticipant);
    DomainParticipantFactory::get_instance()->delete_participant(m_topicsReaderParticipant);
}

void Connector::CreateReader(const string &topicName, const string &topicType)
{
    for (const auto &pair : m_readers) {
        if (pair.second.first == topicName && pair.second.second == topicType) return;
    }

    Topic *topic = m_topicsReaderParticipant->create_topic(topicName, topicType, TOPIC_QOS_DEFAULT);
    m_topics.push_back(topic);
    DataReaderQos rqos = DATAREADER_QOS_DEFAULT;
    rqos.reliability().kind = RELIABLE_RELIABILITY_QOS;
    auto reader = m_subscriber->create_datareader(topic, rqos, &m_messageListener);
    m_readers[reader] = std::make_pair(topicName, topicType);
}

void Connector::SubscribeOnNewTopics(TopicsDiscoveryHandler *handler)
{
    if (find(m_topicsHandlers.begin(), m_topicsHandlers.end(), handler) == m_topicsHandlers.end()) {
        m_topicsHandlers.push_back(handler);
    }
}

void Connector::UnsubscribeFromNewTopics(TopicsDiscoveryHandler *handler)
{
    auto iter = find(m_topicsHandlers.begin(), m_topicsHandlers.end(), handler);
    if (iter < m_topicsHandlers.end()) {
        m_topicsHandlers.erase(iter);
    }
}

void Connector::SubscribeOnMessages(const string &topicName, const string &topicType, MessageHandler *handler)
{
    if (m_handlers.find(topicName) == m_handlers.end()) {
        m_handlers[topicName] = vector<MessageHandler *>();
    }
    m_handlers[topicName].push_back(handler);
    CreateReader(topicName, topicType);
}

void Connector::UnsubscribeFromMessages(const string &name, MessageHandler *handler)
{
    auto iter = find(m_handlers[name].begin(), m_handlers[name].end(), handler);
    if (iter < m_handlers[name].end()) {
        m_handlers[name].erase(iter);
    }
    for (const auto &pair:m_readers) {
        if (pair.second.first == name) {
            m_subscriber->delete_datareader(pair.first);
        }
    }
}

void Connector::MessageListener::on_data_available(DataReader *reader)
{
    SampleInfo info;
    auto topicName = m_connector->m_readers[reader].first;
    auto topicType = m_connector->m_readers[reader].second;
    void *data = m_connector->m_supportedTypes[topicType]->createData();

    if (reader->take_next_sample(data, &info) != ReturnCode_t::RETCODE_OK) return;

    for (auto &handler : m_connector->m_handlers[topicName]) {
        handler->Handle(Ros2Message::Parse(data, topicName, topicType));
    }
    m_connector->m_supportedTypes[topicType].delete_data(data);
}

void Connector::PubListener::on_publisher_discovery(DomainParticipant *participant,
                                                    eprosima::fastrtps::rtps::WriterDiscoveryInfo &&info)
{
    if (info.status == eprosima::fastrtps::rtps::WriterDiscoveryInfo::REMOVED_WRITER) return;
    auto name = info.info.topicName().to_string();
    auto type = info.info.typeName().to_string();

    for (auto *handler: m_connector->m_topicsHandlers) {
        handler->Handle(name, type);
    }
}
