function(install_3rdparty_cmake_project GIT GIT_TAG PROJECT_NAME CMAKE_ARG_LIST)

    set(INSTALL_DIR "${CMAKE_BINARY_DIR}/third_party/install/${PROJECT_NAME}")
    set(BUILD_DIR "${CMAKE_BINARY_DIR}/third_party/build/${PROJECT_NAME}")
    set(SOURCE_DIR "${CMAKE_BINARY_DIR}/third_party/sources/${PROJECT_NAME}")

    set(${PROJECT_NAME}_INSTALL_DIR "${INSTALL_DIR}" PARENT_SCOPE)
    set(${PROJECT_NAME}_BUILD_DIR "${BUILD_DIR}" PARENT_SCOPE)
    set(${PROJECT_NAME}_SOURCE_DIR "${SOURCE_DIR}" PARENT_SCOPE)

    message(STATUS "${PROJECT_NAME}_SOURCE_DIR: ${SOURCE_DIR}")
    message(STATUS "${PROJECT_NAME}_BUILD_DIR: ${BUILD_DIR}")
    message(STATUS "${PROJECT_NAME}_INSTALL_DIR: ${INSTALL_DIR}")
    
    if (NOT EXISTS "${INSTALL_DIR}/${PROJECT_NAME}.label")

        execute_process(
            COMMAND "${CMAKE_COMMAND}" -E make_directory "${INSTALL_DIR}"
            COMMAND "${CMAKE_COMMAND}" -E make_directory "${BUILD_DIR}"
            COMMAND "${CMAKE_COMMAND}" -E make_directory "${SOURCE_DIR}"
        )
        
        message(STATUS "git clone...")
        execute_process(COMMAND git clone -b ${GIT_TAG} ${GIT} ${PROJECT_NAME} WORKING_DIRECTORY "${CMAKE_BINARY_DIR}/third_party/sources/")
        message(STATUS "git clone... OK!")
        message(STATUS "git submodule update...")
        execute_process(COMMAND git submodule update --init WORKING_DIRECTORY "${SOURCE_DIR}")
        message(STATUS "git submodule update... OK!")
        message(STATUS "configure...")
        execute_process(COMMAND "${CMAKE_COMMAND}" -E env "PATH=$ENV{PATH}" 
            "${CMAKE_COMMAND}"
            -G "${CMAKE_GENERATOR}" 
            -S "${SOURCE_DIR}"
            -B "${BUILD_DIR}"
            "-DCMAKE_INSTALL_PREFIX=${INSTALL_DIR}"
            ${${CMAKE_ARG_LIST}})
        message(STATUS "configure... OK!")
        message(STATUS "build...")
        execute_process(COMMAND "${CMAKE_COMMAND}" --build "${BUILD_DIR}" --config ${CMAKE_BUILD_TYPE})
        message(STATUS "build... OK!")
        message(STATUS "install...")
        execute_process(COMMAND "${CMAKE_COMMAND}" --install "${BUILD_DIR}" --config ${CMAKE_BUILD_TYPE})
        message(STATUS "install... OK!")

    endif()

    file(TOUCH "${INSTALL_DIR}/${PROJECT_NAME}.label")

endfunction()