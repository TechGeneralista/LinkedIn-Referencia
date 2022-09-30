#pragma once

#include <string>
#include <string_view>
#include <memory>
#include "NonCopyable.hpp"

namespace af
{
    struct LogMessage
    {
        enum class LogLevel
        {
            Information,
            Warning,
            Error
        };

        const LogLevel LogLevel;
        const std::string SourceName;
        const std::string Message;
    };

    class ILog : public NonCopyable
    {
    public:
        virtual ~ILog() {};
        virtual void Print(const LogMessage& logMessage) = 0;
        void TestHU();

    protected:
        static const std::string_view InformationLabel;
        static const std::string_view WarningLabel;
        static const std::string_view ErrorLabel;
        static const std::string_view sourceMessageSeparator;
    };
}
