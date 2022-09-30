set(THIS_NAME "ApplicationFramework")
file(GLOB_RECURSE IncludeFiles "inc/*.hpp")
file(GLOB_RECURSE SourceFiles "src/*.cpp")

add_library(${THIS_NAME} ${IncludeFiles} ${SourceFiles} "${CMAKE_CURRENT_SOURCE_DIR}/../../Externals/glad/src/glad.c")

target_compile_definitions(${THIS_NAME} PUBLIC GLFW_INCLUDE_NONE GLM_FORCE_RADIANS GLM_FORCE_DEPTH_ZERO_TO_ONE)

if(WIN32)

    target_include_directories(${THIS_NAME} PUBLIC 
    "inc/"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../Externals/glfw-3.3.6.bin.WIN64/include"
    "${CMAKE_CURRENT_SOURCE_DIR}/../../Externals/glad/include")

elseif(UNIX)

    target_include_directories(${THIS_NAME} PUBLIC 
    "inc/")

endif()
