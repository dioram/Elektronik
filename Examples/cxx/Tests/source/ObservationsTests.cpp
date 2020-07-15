#include "ObservationsTests.h"

using namespace Elektronik::Common::Data::Pb;

void ObservationsTests::SetUp()
{
    TestsBase::SetUp();

    std::vector<PointPb> pts;

    for (size_t i = 0; i < 5; ++i) {
        PointPb pt;
        pt.set_id(i);
        pt.set_message(std::to_string(i));
        pts.push_back(pt);
    }

    Vector3Pb pos; pos.set_x(0); pos.set_y(.5f);
    pts[0].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(.5f); pos.set_y(-.5f);
    pts[1].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(-.5f); pos.set_y(-.5f);
    pts[2].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(-.5f);
    pts[3].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(.5f);
    pts[4].set_allocated_position(new Vector3Pb(pos));

    for (const auto& pt : pts) {
        ObservationPb obs;
        obs.set_allocated_point(new PointPb(pt));
        m_map.push_back(obs);
    }

    ConnectionPb connection;

    connection.set_id1(m_map[0].point().id()); connection.set_id2(m_map[1].point().id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[0].point().id()); connection.set_id2(m_map[2].point().id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[2].point().id()); connection.set_id2(m_map[4].point().id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[1].point().id()); connection.set_id2(m_map[3].point().id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[3].point().id()); connection.set_id2(m_map[4].point().id());
    m_connections.push_back(connection);
}

TEST_F(ObservationsTests, Create) {
    auto observations = new PacketPb_Observations();
    for (const auto pt : m_map) {
        auto newPt = observations->add_data();
        newPt->CopyFrom(pt);
    }
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_add);
    packet.set_allocated_observations(observations);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(ObservationsTests, Update) {
    auto observations = new PacketPb_Observations();
    for (const auto obs : { m_map[2], m_map[4] }) {
        auto position = new Vector3Pb(obs.point().position());
        position->set_z(position->z() + .5f);

        auto point = new PointPb(obs.point());
        point->set_allocated_position(position);

        auto newObs = observations->add_data();
        newObs->CopyFrom(obs);
        newObs->set_allocated_point(point);
    }
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);
    packet.set_allocated_observations(observations);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(ObservationsTests, UpdateConnections) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);
    packet.set_allocated_observations(new PacketPb_Observations());

    auto connections = new PacketPb_Connections();
    connections->set_action(PacketPb_Connections_Action::PacketPb_Connections_Action_add);
    for (const auto c : m_connections) {
        auto c_ = connections->add_data();
        c_->CopyFrom(c);
    }
    packet.set_allocated_connections(connections);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(ObservationsTests, RemoveConnections) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);
    packet.set_allocated_observations(new PacketPb_Observations());

    auto connections = new PacketPb_Connections();
    connections->set_action(PacketPb_Connections_Action::PacketPb_Connections_Action_remove);
    for (const auto& c : { m_connections[0], m_connections[1], }) {
        auto connection = connections->add_data();
        connection->CopyFrom(c);
    }
    packet.set_allocated_connections(connections);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(ObservationsTests, Remove) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_remove);
    auto pts2remove = new PacketPb_Observations();
    for (const auto& obs : { m_map[1], m_map[3] }) {
        auto pt_ = pts2remove->add_data();
        pt_->CopyFrom(obs);
    }
    packet.set_allocated_observations(pts2remove);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(ObservationsTests, Clear) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_clear);
    packet.set_allocated_observations(new PacketPb_Observations());

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}