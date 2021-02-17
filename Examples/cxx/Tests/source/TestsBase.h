#pragma once
#include <gtest/gtest.h>
#include <grpc++/grpc++.h>
#include <Elektronik/UpdateMapService.grpc.pb.h>

class TestsBase : public ::testing::Test
{
protected:
	
	virtual void SetUp() override
	{
		auto channel = grpc::CreateChannel("127.0.0.1:5050", grpc::InsecureChannelCredentials());
		auto client = Elektronik::Common::Data::Pb::MapsManagerPb::NewStub(channel);
		m_client.swap(client);
	}

	std::unique_ptr<Elektronik::Common::Data::Pb::MapsManagerPb::Stub> m_client;
};