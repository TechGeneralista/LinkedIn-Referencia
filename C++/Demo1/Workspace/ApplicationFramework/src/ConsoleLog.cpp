#include "ConsoleLog.hpp"
#include <iostream>

#ifdef _WIN64
#include <windows.h>
#endif

using std::cout;
using std::endl;

namespace af
{
    ConsoleLog::ConsoleLog()
    {
#ifdef _WIN64
        consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        SetConsoleOutputCP(65001);
#else
        std::locale::global(std::locale("en_US.UTF-8"));
#endif
    }

    ConsoleLog::~ConsoleLog()
    {
        Reset();
    }

    void ConsoleLog::Reset()
    {
#ifdef _WIN64
        SetConsoleTextAttribute(consoleHandle, 7);
#else
        cout << "\033[0m";
#endif
    }

    void ConsoleLog::Print(const LogMessage& logMessage)
    {
        if (logMessage.LogLevel == LogMessage::LogLevel::Information) // Green
        {
#ifdef _WIN64
            SetConsoleTextAttribute(consoleHandle, 2);
#else
            cout << "\033[32m";
#endif
            cout << InformationLabel;
        }

        else if (logMessage.LogLevel == LogMessage::LogLevel::Warning) // Yellow
        {
#ifdef _WIN64
            SetConsoleTextAttribute(consoleHandle, 6);
#else
            cout << "\033[33m";
#endif
            cout << WarningLabel;
        }

        else if (logMessage.LogLevel == LogMessage::LogLevel::Error) // Red
        {
#ifdef _WIN64
            SetConsoleTextAttribute(consoleHandle, 4);
#else
            cout << "\033[31m";
#endif
            cout << ErrorLabel;
        }

        Reset();
        cout << logMessage.SourceName;

        if (!logMessage.Message.empty())
        {
            cout << sourceMessageSeparator << logMessage.Message;
        }

        cout << endl;
    }
}
