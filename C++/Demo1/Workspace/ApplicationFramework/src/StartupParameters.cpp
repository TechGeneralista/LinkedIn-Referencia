#include "StartupParameters.hpp"

using namespace std;

namespace af
{
	void StartupParameters::Parse(int argc, char** argv)
	{
		Parameters = vector<string>(argv, argv + argc);
		Log = make_shared<ConsoleLog>();
	}

	bool StartupParameters::IsEmpty()
	{
		return Parameters.size() != 1;
	}
}
