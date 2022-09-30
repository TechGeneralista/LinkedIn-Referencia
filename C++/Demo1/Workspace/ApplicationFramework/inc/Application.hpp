#pragma once

#include "NonCopyable.hpp"
#include "ApplicationDescription.hpp"
#include <memory>
#include <vector>
#include "Window.hpp"

namespace af
{
	class Application : public NonCopyable
	{
	public:
		std::shared_ptr<ApplicationDescription> Description;

		Application(std::shared_ptr<ApplicationDescription> appDesc);
		~Application();

		void AddWindow(std::shared_ptr<Window> window);
		void Run();

	private:
		std::vector<std::shared_ptr<Window>> windows;

		void InitGLFW();
	};
}
