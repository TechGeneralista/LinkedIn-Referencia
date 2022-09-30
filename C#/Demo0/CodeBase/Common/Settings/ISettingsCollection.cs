namespace Common.Settings
{
    public interface ISettingsCollection
    {
        ISettingsStore SettingsStore { get; }
        int Count { get; }
        string[] Keys { get; }
        bool IsErrorOccured { get; }
        KeyCreator KeyCreator { get; }


        void SetValue<T>(T value);
        T GetValue<T>();
        T GetValue<T>(T defaultValue);

        void SetValue<T>(string key, T value);
        T GetValue<T>(string key);
        T GetValue<T>(string key, T defaultValue);

        void ErrorOccurred();

        void Write();
        void Read();
    }
}