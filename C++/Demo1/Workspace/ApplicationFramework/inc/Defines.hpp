#pragma once

// Debug build detektálása
#if (defined __GNUC__ && !defined NDEBUG) || (defined _MSC_VER && defined _DEBUG)
#define AFDEBUG
#endif
