#pragma once

#include "NonCopyable.hpp"
#include <memory>
#include "Log.hpp"
#include <functional>
#include "StartupParameters.hpp"

namespace af
{
	class ExceptionHandler : public NonCopyable
	{
	public:
		std::shared_ptr<StartupParameters> Parameters;

		ExceptionHandler(int argc, char** argv);
		void Invoke(std::function<void(std::shared_ptr<StartupParameters>)> handledMain);
	};
}
