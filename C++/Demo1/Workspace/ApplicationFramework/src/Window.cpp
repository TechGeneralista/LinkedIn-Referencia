#include "Window.hpp"
#include "glad/glad.h"
#include "Log.hpp"

using namespace std;

namespace af
{
	bool Window::gladLoaded = false;

	Window::Window()
	{
		windowHandle = glfwCreateWindow(800, 600, "", NULL, NULL);
		closeFlag = false;
		ClearColor = { 255,0,0,255 };
	}

	Window::~Window()
	{
		glfwDestroyWindow(windowHandle);
	}

	bool Window::ApplicationLoop()
	{
		if (closeFlag)
			return closeFlag;

		glfwMakeContextCurrent(windowHandle);

		if (!gladLoaded)
		{
			if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
				LOG_THROW_ERROR("Window::Window(string title)", "!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress)");

			gladLoaded = !gladLoaded;
		}

		int width; int height;
		glfwGetWindowSize(windowHandle, &width, &height);
		glViewport(0, 0, width, height);

		glClearColor
		(
			static_cast<float>(ClearColor.Red) / 255.0f, 
			static_cast<float>(ClearColor.Green) / 255.0f, 
			static_cast<float>(ClearColor.Blue) / 255.0f,
			static_cast<float>(ClearColor.Alpha) / 255.0f
		);
		
		glClear(GL_COLOR_BUFFER_BIT);

		glfwSwapBuffers(windowHandle);
		glfwPollEvents();

		if (Loop)
		{
			bool close = Loop(this);

			if (close)
				return close;
		}

		return glfwWindowShouldClose(windowHandle);
	}

	void Window::Show()
	{
		glfwShowWindow(windowHandle);
	}

	void Window::Hide()
	{
		glfwHideWindow(windowHandle);
	}

	void Window::Close()
	{
		closeFlag = true;
	}
}
