#include "Version.hpp"
#include "Defines.hpp"

using namespace std;

namespace af
{
	const char Version::Separator = '.';

	string Version::ToString()
	{
		return to_string(Major) + Separator + to_string(Minor) + Separator + to_string(Build);
	}

	string Version::ToStringDebug()
	{
#ifndef AFDEBUG
		return ToString();
#else
		return to_string(Major) + Separator + to_string(Minor) + Separator + "Debug";
#endif
	}
}
