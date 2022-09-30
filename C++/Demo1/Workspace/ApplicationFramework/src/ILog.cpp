#include "ILog.hpp"

namespace af
{
	const std::string_view ILog::InformationLabel{"i: "};
	const std::string_view ILog::WarningLabel{"w: "};
	const std::string_view ILog::ErrorLabel{"e: "};
	const std::string_view ILog::sourceMessageSeparator{" -> "};

	void ILog::TestHU()
	{
		this->Print(LogMessage{ LogMessage::LogLevel::Information, "SimpleLogTestHU", "Magyar ékezetes szöveg teszt:" });
		this->Print(LogMessage{ LogMessage::LogLevel::Warning, "SimpleLogTestHU", "Árvíztűrő tükörfúrógép" });
		this->Print(LogMessage{ LogMessage::LogLevel::Error, "SimpleLogTestHU", "Öt szép szűzlány őrült írót nyúz" });
	}
}
