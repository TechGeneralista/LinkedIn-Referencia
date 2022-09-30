#pragma once

#include "ILog.hpp"
#include "ConsoleLog.hpp"

#define LOG_INFORMATION(instance, source, message) instance->Print(af::LogMessage{ af::LogMessage::LogLevel::Information, source, message})
#define LOG_WARNING(instance, source, message) instance->Print(af::LogMessage{ af::LogMessage::LogLevel::Warning, source, message})
#define LOG_ERROR(instance, source, message) instance->Print(af::LogMessage{ af::LogMessage::LogLevel::Error, source, message})
#define LOG_THROW_ERROR(source, message) throw af::LogMessage{ af::LogMessage::LogLevel::Error, source, message}

#ifdef _WIN64
#define LOG_THROW_ERROR_WITH_LOCATION(message) LOG_THROW_ERROR(__FUNCSIG__, message)
#else
#define LOG_THROW_ERROR_WITH_LOCATION(message) LOG_THROW_ERROR(__PRETTY_FUNCTION__, message)
#endif
