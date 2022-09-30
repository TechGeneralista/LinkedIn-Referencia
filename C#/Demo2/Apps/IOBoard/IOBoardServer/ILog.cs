using System;


namespace IOBoardServer
{
    public enum LogLevels { Information, Warning, Error}

    public interface ILog
    {
        event Action<string> ErrorOccurred;

        void NewMessage(LogLevels logLevel, string message);
    }
}