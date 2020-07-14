cmake_minimum_required(VERSION 3.15)
project(cxx.Tests)
set(TARGET_NAME ${PROJECT_NAME})

###############################################################
# Generate protoc files
###############################################################

find_program(PROTOC_PATH protoc)
find_program(GRPC_PLUGIN_PATH grpc_cpp_plugin)

set(GENERATED_SOURCE_FILES_DIR ${CMAKE_CURRENT_BINARY_DIR}/generated)
file(MAKE_DIRECTORY "${GENERATED_SOURCE_FILES_DIR}/Elektronik")

file(GLOB_RECURSE PROTO_FILES "../../../Assets/Scripts/Common/Data/Protobuf/*.proto")

find_package(Protobuf CONFIG REQUIRED)
get_target_property(PROTOBUF_INCLUDE_DIR protobuf::libprotobuf INTERFACE_INCLUDE_DIRECTORIES)

foreach(PROTO_FILE IN LISTS PROTO_FILES)
	get_filename_component(FILE_DIRECTORY ${PROTO_FILE} DIRECTORY)
	execute_process(COMMAND "${PROTOC_PATH}" 
		"-I${PROTOBUF_INCLUDE_DIR}"
		"-I${FILE_DIRECTORY}" 
		"--plugin=protoc-gen-grpc=${GRPC_PLUGIN_PATH}" 
		"--cpp_out=${GENERATED_SOURCE_FILES_DIR}/Elektronik" 
		"--grpc_out=${GENERATED_SOURCE_FILES_DIR}/Elektronik"
		"${PROTO_FILE}"
	ERROR_VARIABLE ERROR)
	if(ERROR)
		message(FATAL_ERROR "${ERROR}")
	endif()
endforeach()

###############################################################
# Target
###############################################################

file(GLOB_RECURSE GENERATED_CPP_FILES "${GENERATED_SOURCE_FILES_DIR}/*.cpp")
file(GLOB_RECURSE GENERATED_CC_FILES "${GENERATED_SOURCE_FILES_DIR}/*.cc")

set(GENERATED_FILES ${GENERATED_CPP_FILES} ${GENERATED_CC_FILES})

set(SOURCE_FILES 
	source/main.cpp
	source/ObservationsTests.cpp
	source/PointsStressTests.cpp
	source/PointsTests.cpp
	source/TrackedObjsTests.cpp )

add_executable(${TARGET_NAME} ${SOURCE_FILES} ${GENERATED_FILES})

target_compile_definitions(${TARGET_NAME} PRIVATE "-D_WIN32_WINNT=0x600")
target_include_directories(${TARGET_NAME} PRIVATE ${GENERATED_SOURCE_FILES_DIR})

find_package(Protobuf CONFIG REQUIRED)
target_link_libraries(${TARGET_NAME} PRIVATE protobuf::libprotobuf)

find_package(gRPC CONFIG REQUIRED)
target_link_libraries(${TARGET_NAME} PRIVATE gRPC::grpc++)

find_package(GTest CONFIG REQUIRED)
target_link_libraries(${TARGET_NAME} PRIVATE GTest::gtest GTest::gtest_main)