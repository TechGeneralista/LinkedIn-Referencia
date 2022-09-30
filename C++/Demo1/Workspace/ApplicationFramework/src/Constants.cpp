#include "Constants.hpp"

using namespace std;

namespace af
{
#ifdef _WIN64

	const char PathSeparator = '\\';
	const string PathAppsConfigKey = "APPDATA";

#else

	const char PathSeparator = '/';
	const string PathAppsConfigKey = "HOME";

#endif
}
