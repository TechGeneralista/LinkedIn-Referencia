#include "ApplicationDescription.hpp"
#include "Log.hpp"
#include <filesystem>
#include "Constants.hpp"

using namespace std;
using namespace std::filesystem;

namespace af
{
	void ApplicationDescription::CreateDirectoriesIfNotExists()
	{
		string pathAppsConfig = GetPathAppsConfig();
		PathAppConfig = pathAppsConfig + PathSeparator + Name + PathSeparator + Version.ToStringDebug();

		if (!exists(PathAppConfig))
		{
			create_directories(PathAppConfig);

			if (!is_directory(PathAppConfig))
				LOG_THROW_ERROR("ApplicationDescription::CreateDirectoriesIfNotExists()", "!is_directory(PathAppConfig)");
		}
	}

	std::string ApplicationDescription::GetPathAppsConfig()
	{
#pragma warning( disable : 4996 )
		char* temp = std::getenv(PathAppsConfigKey.c_str());

		if (temp == nullptr)
			LOG_THROW_ERROR("ApplicationDescription::GetPathAppsConfig()", "temp == nullptr");

		return temp;
	}
}
