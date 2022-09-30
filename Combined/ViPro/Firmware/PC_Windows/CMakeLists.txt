cmake_minimum_required(VERSION 3.24.0)

project("RunnerCore")

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(EXEC_NAME "RunnerCore_PCWindows")

file(GLOB_RECURSE INC_FILES
"${CMAKE_CURRENT_SOURCE_DIR}/../RunnerCore/inc/*.hpp"
"${CMAKE_CURRENT_SOURCE_DIR}/inc/*.hpp")

file(GLOB_RECURSE SRC_FILES
"${CMAKE_CURRENT_SOURCE_DIR}/../RunnerCore/src/*.cpp"
"${CMAKE_CURRENT_SOURCE_DIR}/src/*.cpp")

add_executable(${EXEC_NAME} ${INC_FILES} ${SRC_FILES})

target_include_directories(${EXEC_NAME} PRIVATE
"${CMAKE_CURRENT_SOURCE_DIR}/../RunnerCore/inc/"
"${CMAKE_CURRENT_SOURCE_DIR}/inc/")
