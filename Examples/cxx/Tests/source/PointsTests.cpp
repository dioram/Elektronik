#include "PointsTests.h"

using namespace Elektronik::Common::Data::Pb;

void PointsTests::SetUp()
{
	TestsBase::SetUp();
    for (size_t i = 0; i < 5; ++i) {
        PointPb pt; 
        pt.set_id(i);
        pt.set_message(std::to_string(i));
        m_map.push_back(pt);
    }

    Vector3Pb pos; pos.set_x(0); pos.set_y(.5f);
    m_map[0].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(.5f); pos.set_y(-.5f);
    m_map[1].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(-.5f); pos.set_y(-.5f);
    m_map[2].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(-.5f);
    m_map[3].set_allocated_position(new Vector3Pb(pos));

    pos.Clear(); pos.set_x(.5f);
    m_map[4].set_allocated_position(new Vector3Pb(pos));

    ConnectionPb connection; 
    
    connection.set_id1(m_map[0].id()); connection.set_id2(m_map[1].id());
    m_connections.push_back(connection);
    
    connection.set_id1(m_map[0].id()); connection.set_id2(m_map[2].id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[2].id()); connection.set_id2(m_map[4].id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[1].id()); connection.set_id2(m_map[3].id());
    m_connections.push_back(connection);

    connection.set_id1(m_map[3].id()); connection.set_id2(m_map[4].id());
    m_connections.push_back(connection);
}

TEST_F(PointsTests, Create) {
    auto points = new PacketPb_Points();
    for (const auto pt : m_map) {
        auto newPt = points->add_data();
        newPt->CopyFrom(pt);
    }
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_add);
    packet.set_allocated_points(points);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(PointsTests, Update) {
    auto points = new PacketPb_Points();
    for (const auto pt : { m_map[2], m_map[4] }) {
        auto position = new Vector3Pb(pt.position());
        position->set_z(position->z() + .5f);
        auto newPt = points->add_data();
        newPt->CopyFrom(pt);
        newPt->set_allocated_position(position);
    }
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);
    packet.set_allocated_points(points);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(PointsTests, UpdateConnections) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);
    packet.set_allocated_points(new PacketPb_Points());

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

TEST_F(PointsTests, RemoveConnections) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);
    packet.set_allocated_points(new PacketPb_Points());

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

TEST_F(PointsTests, Remove) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_remove);
    auto pts2remove = new PacketPb_Points();
    for (const auto& pt : { m_map[1], m_map[3] }) {
        auto pt_ = pts2remove->add_data();
        pt_->set_id(pt.id());
    }
    packet.set_allocated_points(pts2remove);

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}

TEST_F(PointsTests, Clear) {
    PacketPb packet;
    packet.set_action(PacketPb_ActionType::PacketPb_ActionType_clear);
    packet.set_allocated_points(new PacketPb_Points());

    grpc::ClientContext ctx;
    ErrorStatusPb status;
    m_client->Handle(&ctx, packet, &status);

    ASSERT_EQ(status.err_type(), ErrorStatusPb_ErrorStatusEnum::ErrorStatusPb_ErrorStatusEnum_SUCCEEDED);
}