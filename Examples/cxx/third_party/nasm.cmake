set(DOWNLOAD_DIR "${CMAKE_BINARY_DIR}/third_party/downloads/nasm")
set(INSTALL_DIR "${CMAKE_BINARY_DIR}/third_party/install/nasm")

file(MAKE_DIRECTORY "${INSTALL_DIR}")

file(DOWNLOAD https://www.nasm.us/pub/nasm/releasebuilds/2.15.02/win64/nasm-2.15.02-win64.zip ${DOWNLOAD_DIR}/nasm-2.15.02-win64.zip)


execute_process(COMMAND "${CMAKE_COMMAND}" -E tar -xvzf "${DOWNLOAD_DIR}/nasm-2.15.02-win64.zip" nasm-2.15.02
	WORKING_DIRECTORY "${INSTALL_DIR}")

set(CMAKE_PROGRAM_PATH "${INSTALL_DIR}/nasm-2.15.02;${CMAKE_PROGRAM_PATH}")