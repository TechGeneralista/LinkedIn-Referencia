#pragma once

// Debug build detekt�l�sa
#if (defined __GNUC__ && !defined NDEBUG) || (defined _MSC_VER && defined _DEBUG)
#define AFDEBUG
#endif
