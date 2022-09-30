set(THIS_NAME "LearnOpenGL")
file(GLOB_RECURSE IncludeFiles "*.hpp")
file(GLOB_RECURSE SourceFiles "*.cpp")

add_executable (${THIS_NAME} ${IncludeFiles} ${SourceFiles} "${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glad/src/glad.c")

target_include_directories(${THIS_NAME} PRIVATE
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glad/include"
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glfw-3.3.6.bin.WIN64/include"
)

target_link_libraries(${THIS_NAME} 
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glfw-3.3.6.bin.WIN64/lib-vc2022/glfw3.lib"
)
