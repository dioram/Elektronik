include(install_3rdparty)

install_3rdparty_cmake_project(https://github.com/google/googletest.git release-1.10.0 gtest "")

set(CMAKE_PREFIX_PATH "${gtest_INSTALL_DIR};${CMAKE_PREFIX_PATH}")