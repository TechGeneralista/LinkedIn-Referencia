#include "ExceptionHandler.hpp"

using namespace std;

namespace af
{
	ExceptionHandler::ExceptionHandler(int argc, char** argv)
	{
		Parameters = make_shared<StartupParameters>();
		Parameters->Parse(argc, argv);
		Parameters->Log = make_shared<ConsoleLog>();
	}

	void ExceptionHandler::Invoke(std::function<void(std::shared_ptr<StartupParameters>)> handledMain)
	{
		try
		{
			handledMain(Parameters);
		}
		catch (const LogMessage& errorMessage)
		{
			Parameters->Log->Print(errorMessage);
			Parameters->ReturnValue = EXIT_FAILURE;
		}
		catch (...)
		{
			LOG_ERROR(Parameters->Log, "ExceptionHandler", "Fatal error");
			Parameters->ReturnValue = EXIT_FAILURE;
		}
	}
}
