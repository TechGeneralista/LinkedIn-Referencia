namespace AppLog
{
    public enum LogTypes { Information, Successful, Warning, Error}

    public interface ILog
    {
        void NewMessage(LogTypes logType, string message, string parameter = null);
    }
}
