find_package(Threads REQUIRED)
find_package(Protobuf CONFIG QUIET)
find_package(gRPC CONFIG QUIET)

if (NOT Protobuf_FOUND OR NOT gRPC_FOUND)
	include(${CMAKE_CURRENT_LIST_DIR}/nasm.cmake)
	include(${CMAKE_CURRENT_LIST_DIR}/grpc.cmake)
endif()


find_package(GTest CONFIG QUIET)
if (NOT GTest_FOUND)
	include(${CMAKE_CURRENT_LIST_DIR}/gtest.cmake)
endif()

