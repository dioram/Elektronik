#include <gtest/gtest.h>
#include "PointsTests.h"
#include "ObservationsTests.h"

int main(int argc, char* argv[]) 
{
	::testing::InitGoogleTest(&argc, argv);
	return RUN_ALL_TESTS();
}