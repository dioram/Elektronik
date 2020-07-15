#include "OfflineTests.h"
#include <fstream>
#include <google/protobuf/util/delimited_message_util.h>
#include <Elektronik/MapTypes.pb.h>

using namespace Elektronik::Common::Data::Pb;

TEST_F(OfflineTests, ReadWriteTest)
{
	std::ofstream output_stream("packets.data", std::ios::binary);

	for (int i = 0; i < 10; ++i)
	{
		PacketPb packet;
		packet.set_action(PacketPb_ActionType::PacketPb_ActionType_update);

		auto pts = new PacketPb_Points();
		for (size_t i = 0; i < 100; ++i) {
			// set new point id
			pts->add_data()->set_id(i);
		}
		// this also set packet type, in this case it is points
		packet.set_allocated_points(pts);

		google::protobuf::util::SerializeDelimitedToOstream(packet, &output_stream);
		
	}

	output_stream.close();
	ASSERT_TRUE(true);

	std::ifstream input_stream("packets.data", std::ios::binary);
	google::protobuf::io::IstreamInputStream istream(&input_stream);
	google::protobuf::io::CodedInputStream coded_istream(&istream);

	bool eof = false;
	int packetNum = 0;
	for (int i = 0; i < 10; ++i)
	{
		while (!eof) {
			PacketPb packet;
			google::protobuf::util::ParseDelimitedFromCodedStream(&packet, &coded_istream, &eof);
			if (!eof) {
				ASSERT_EQ(packet.points().data_size(), 100);
				ASSERT_EQ(packet.action(), PacketPb_ActionType::PacketPb_ActionType_update);
				++packetNum;
			}
		}
	}
	ASSERT_EQ(packetNum, 10);

	input_stream.close();
}