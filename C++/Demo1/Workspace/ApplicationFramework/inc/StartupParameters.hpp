#pragma once

#include <vector>
#include <string>
#include "NonCopyable.hpp"
#include <memory>
#include "Log.hpp"

namespace af
{
	struct StartupParameters : public NonCopyable
	{
		std::vector<std::string> Parameters;
		std::shared_ptr<ILog> Log;
		int ReturnValue = EXIT_SUCCESS;

		void Parse(int argc, char** argv);
		bool IsEmpty();
	};
}
