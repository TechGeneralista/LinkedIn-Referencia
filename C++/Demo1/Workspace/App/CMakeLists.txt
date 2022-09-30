set(THIS_NAME "App")
file(GLOB_RECURSE IncludeFiles "inc/*.hpp")
file(GLOB_RECURSE SourceFiles "src/*.cpp")

add_executable (${THIS_NAME} ${IncludeFiles} ${SourceFiles})

add_custom_command(
	TARGET ${THIS_NAME} 
	PRE_BUILD 
	COMMAND ${CMAKE_COMMAND} -P "${CMAKE_CURRENT_LIST_DIR}/buildNumberIncrementScript.cmake"
)

target_include_directories(${THIS_NAME} PRIVATE "inc/")

if(WIN32)

	target_link_libraries(${THIS_NAME} 
	ApplicationFramework
	"${CMAKE_CURRENT_SOURCE_DIR}/../../Externals/glfw-3.3.6.bin.WIN64/lib-vc2022/glfw3.lib")

elseif(UNIX)

	target_link_libraries(${THIS_NAME} 
	ApplicationFramework)

endif()
