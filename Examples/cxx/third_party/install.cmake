find_package(Threads REQUIRED)

find_package(GTest CONFIG QUIET)
if (NOT GTest_FOUND)
	include(${CMAKE_CURRENT_LIST_DIR}/gtest.cmake)
endif()

find_program(NASM_EXECUTABLE nasm)
if (${NASM_EXECUTABLE} STREQUAL "NASM_EXECUTABLE-NOTFOUND")
include(${CMAKE_CURRENT_LIST_DIR}/nasm.cmake)
endif()

find_package(Protobuf CONFIG QUIET)
find_package(gRPC CONFIG QUIET)
if (NOT gRPC_FOUND)
	include(${CMAKE_CURRENT_LIST_DIR}/grpc.cmake)
endif()