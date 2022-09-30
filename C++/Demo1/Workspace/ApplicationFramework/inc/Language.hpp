#pragma once

#include "NonCopyable.hpp"
#include <string>
#include <vector>

namespace af
{
	class Language : public NonCopyable
	{
	public:
		std::string Name;
		std::vector<std::string> Texts;
		void Save(std::string path);
	};
}
