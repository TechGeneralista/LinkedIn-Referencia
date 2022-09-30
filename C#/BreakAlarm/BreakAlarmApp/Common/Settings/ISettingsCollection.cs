namespace Common.Settings
{
    public interface ISettingsCollection
    {
        int Count { get; }
        string[] Keys { get; }


        void SetValue<T>(string key, T value);
        T GetValue<T>(string key);
        T GetValue<T>(string key, T defaultValue);
        void Write();
    }
}