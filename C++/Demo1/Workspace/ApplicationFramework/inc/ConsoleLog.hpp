#pragma once

#include "ILog.hpp"
#include <memory>

#ifdef _WIN64
#include <Windows.h>
#endif

namespace af
{
    class ConsoleLog : public ILog
    {
    public:
        ConsoleLog();
        virtual ~ConsoleLog();
        virtual void Print(const LogMessage& logMessage);

    private:
        void Reset();
#ifdef _WIN64
        HANDLE consoleHandle;
#endif
    };
}
