#include "Language.hpp"
#include <fstream>
#include "Log.hpp"
#include <filesystem>
#include "Constants.hpp"

using namespace std;
using namespace std::filesystem;

namespace af
{
	void Language::Save(string path)
	{
		string pathLanguage = path + PathSeparator + "Languages";

		if (!exists(pathLanguage))
		{
			create_directories(pathLanguage);

			if(!is_directory(pathLanguage))
				LOG_THROW_ERROR("Language::Save(string path)", "!is_directory(pathLanguage)");
		}

		string pathFile = pathLanguage + PathSeparator + Name + ".txt";
		ofstream file{ pathFile, ios::out };

		if (file.is_open())
		{
			for (const auto& line : Texts)
				file << line << endl;

			file.close();
		}
		else
			LOG_THROW_ERROR("Language::Save(string path)", "!file.is_open()");
	}
}
