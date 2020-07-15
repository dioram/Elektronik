include(install_3rdparty)

set(ARGS "-Dgtest_force_shared_crt=ON")

install_3rdparty_cmake_project(https://github.com/google/googletest.git release-1.10.0 gtest ARGS)

set(CMAKE_PREFIX_PATH "${gtest_INSTALL_DIR};${CMAKE_PREFIX_PATH}")