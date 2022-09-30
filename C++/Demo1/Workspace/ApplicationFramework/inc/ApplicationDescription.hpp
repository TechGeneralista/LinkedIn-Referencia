#pragma once

#include "NonCopyable.hpp"
#include <string>
#include "Version.hpp"
#include <memory>
#include "Language.hpp"

namespace af
{
	class ApplicationDescription : public NonCopyable
	{
	public:
		std::string Name;
		Version Version;
		std::shared_ptr<Language> DefaultLanguage;
		std::string PathAppConfig;

		void CreateDirectoriesIfNotExists();

	private:
		std::string GetPathAppsConfig();
	};
}
