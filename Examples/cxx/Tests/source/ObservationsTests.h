#pragma once
#include "TestsBase.h"
#include <Elektronik/UpdateMapService.grpc.pb.h>

class ObservationsTests : public TestsBase
{
protected:
	virtual void SetUp() override;

protected:
	std::vector<Elektronik::Common::Data::Pb::ObservationPb> m_map;
	std::vector<Elektronik::Common::Data::Pb::ConnectionPb> m_connections;
};