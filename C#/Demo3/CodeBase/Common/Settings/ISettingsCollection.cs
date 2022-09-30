namespace Common.Settings
{
    public interface ISettingsCollection
    {
        ISettingsStore SettingsStore { get; }
        bool IsErrorOccured { get; }


        void AddKey(params string[] keys);
        void RemoveLastKey(int count = 1);
        void ReplaceLastKey(string key);

        void SetValue<T>(string key, T value);

        T GetValue<T>(string key);
        T GetValue<T>(string key, T defaultValue);

        void ErrorOccurred();

        void Write();
        void Read();

        string GetNextKey();
        void EntryPoint(string name);
        void SetProperty<T>(T propertyValue, params string[] propertyNames);
        T GetProperty<T>(params string[] propertyNames);
        void ExitPoint();
    }
}