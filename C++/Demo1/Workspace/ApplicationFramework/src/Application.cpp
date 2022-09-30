#include "Application.hpp"
#include "Log.hpp"

using namespace std;

namespace af
{

	Application::Application(shared_ptr<ApplicationDescription> appDesc)
	{
		Description = appDesc;
		Description->CreateDirectoriesIfNotExists();
		Description->DefaultLanguage->Save(Description->PathAppConfig);

		InitGLFW();
	}

	Application::~Application()
	{
		glfwTerminate();
	}

	void Application::InitGLFW()
	{
		if (!glfwInit())
			LOG_THROW_ERROR("void Application::InitGLFW()", "!glfwInit()");
		
		glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
		glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 6);
		glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
		glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE);
		glfwWindowHint(GLFW_TRANSPARENT_FRAMEBUFFER, GLFW_TRUE);

#ifdef __APPLE__
		glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
#endif
	}

	void Application::AddWindow(std::shared_ptr<Window> window)
	{
		windows.push_back(window);
	}

	void Application::Run()
	{
		bool closeFlag;
		int indexToRemove;

		while (windows.size() > 0)
		{
			closeFlag = false;
			indexToRemove = -1;
			for (const auto& window : windows)
			{
				indexToRemove += 1;
				closeFlag = window->ApplicationLoop();

				if (closeFlag)
					break;
			}

			if (closeFlag)
				windows.erase(windows.begin() + indexToRemove);
		}
	}
}
