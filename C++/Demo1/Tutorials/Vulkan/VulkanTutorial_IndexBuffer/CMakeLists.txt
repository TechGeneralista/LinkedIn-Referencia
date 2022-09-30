add_executable(VulkanTutorial_IndexBuffer "main.cpp")

target_include_directories(VulkanTutorial_IndexBuffer PRIVATE 
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glfw-3.3.6.bin.WIN64/include"
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glm/include"
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/VulkanSDK_1.2.198.1/include"
)

target_link_libraries(VulkanTutorial_IndexBuffer 
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/glfw-3.3.6.bin.WIN64/lib-vc2022/glfw3.lib"
"${CMAKE_CURRENT_SOURCE_DIR}/../../../Externals/VulkanSDK_1.2.198.1/Lib/vulkan-1.lib"
)
