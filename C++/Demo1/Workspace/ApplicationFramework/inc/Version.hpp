#pragma once

#include "NonCopyable.hpp"
#include <cstdint>
#include <string>

namespace af
{
	class Version : public NonCopyable
	{
	public:
		uint8_t Major;
		uint16_t Minor;
		uint32_t Build;

		std::string ToString();
		std::string ToStringDebug();

	private:
		static const char Separator;
	};
}
