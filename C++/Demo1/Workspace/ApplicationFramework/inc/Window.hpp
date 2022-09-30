#pragma once

#include "NonCopyable.hpp"
#include <GLFW/glfw3.h>
#include <string>
#include <functional>
#include "ColorRGBA.hpp"

namespace af
{
	class Window : public NonCopyable
	{
	public:
		ColorRGBA ClearColor;

		std::function<bool(Window* window)> Loop;

		Window();
		~Window();

		bool ApplicationLoop();
		void Show();
		void Hide();
		void Close();

	private:
		static bool gladLoaded;
		GLFWwindow* windowHandle;
		bool closeFlag;
	};
}
