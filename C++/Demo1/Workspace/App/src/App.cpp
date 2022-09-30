#include "App.hpp"
#include "StartupParameters.hpp"
#include "ExceptionHandler.hpp"
#include "Log.hpp"
#include "ApplicationDescription.hpp"
#include "Language.hpp"
#include "Application.hpp"
#include "Window.hpp"

using namespace af;
using namespace std;

shared_ptr<Language> GetDefaultLanguage()
{
	shared_ptr<Language> defLang = make_shared<Language>();
	defLang->Name = "English";
	defLang->Texts.push_back("Application started");
	defLang->Texts.push_back("Application ended");

	return defLang;
}

bool WinLoop(Window* window)
{
	

	return false;
}

void handledMain(shared_ptr<StartupParameters> parameters)
{
	shared_ptr<Language> defaultLanguage = GetDefaultLanguage();
	LOG_INFORMATION(parameters->Log, "void handledMain(shared_ptr<StartupParameters> parameters)", defaultLanguage->Texts[0]); // Application started

	shared_ptr<ApplicationDescription> appDesc = make_shared<ApplicationDescription>();
	appDesc->Name = "App";
	appDesc->Version.Major = 4;
	appDesc->Version.Minor = 0;
	appDesc->Version.Build = BUILD_NUMBER;
	appDesc->DefaultLanguage = defaultLanguage;

	shared_ptr<Application> app = make_shared<Application>(appDesc);

	shared_ptr<Window> window = make_shared<Window>();
	window->Loop = WinLoop;
	window->Show();

	app->AddWindow(window);
	app->Run();

	LOG_INFORMATION(parameters->Log, "void handledMain(shared_ptr<StartupParameters> parameters)", defaultLanguage->Texts[1]); // Application ended
}

int main(int argc, char** argv)
{
	ExceptionHandler exceptionHandler{argc, argv};
	exceptionHandler.Invoke(handledMain);
	return exceptionHandler.Parameters->ReturnValue;
}
