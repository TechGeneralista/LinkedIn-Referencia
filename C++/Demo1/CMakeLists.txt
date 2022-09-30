# CMakeList.txt : Top-level CMake project file, do global configuration
# and include sub-projects here.
# A Visual Studioban ezt a root mappát megnyitva detektálja a CMAKE projektet
cmake_minimum_required (VERSION 3.8)

# Gyűjtőprojekt
project ("Future")

# Ez az összes alprojektre beállítja a szabványt
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Munkakönyvtár
add_subdirectory("Workspace/App")
add_subdirectory("Workspace/ApplicationFramework")

# Példák
#add_subdirectory("Test") # szimpla projekt

# Vulkan
#add_subdirectory("Tutorials/Vulkan/VulkanTutorial_IndexBuffer") # https://vulkan-tutorial.com/Vertex_buffers/Index_buffer
#add_subdirectory("Tutorials/Vulkan/VulkanTutorial_StagingBuffer_CPP") # https://github.com/bwasty/vulkan-tutorial-hpp/blob/master/steps/19_staging_buffer.cpp

# OpenGL
add_subdirectory("Tutorials/OpenGL/LearnOpenGL") # https://learnopengl.com/Getting-started/Shaders
