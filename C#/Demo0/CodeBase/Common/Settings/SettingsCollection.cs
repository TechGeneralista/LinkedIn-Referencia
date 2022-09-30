using System.Collections.Generic;
using System.Linq;


namespace Common.Settings
{
    public class SettingsCollection : ISettingsCollection
    {
        public ISettingsStore SettingsStore { get; }
        public bool IsErrorOccured { get; private set; }
        public KeyCreator KeyCreator { get; } = new KeyCreator();
        public int Count => keyValues.Keys.Count;
        public string[] Keys => keyValues.Keys.ToArray();


        Dictionary<string, object> keyValues = new Dictionary<string, object>();


        public SettingsCollection(ISettingsStore settingsStore, bool read = false)
        {
            SettingsStore = settingsStore;

            if (read)
                Read();
        }


        public void SetValue<T>(T value) => SetValue<T>(KeyCreator.Key, value);
        public T GetValue<T>() => GetValue<T>(KeyCreator.Key);
        public T GetValue<T>(T defaultValue) => GetValue<T>(KeyCreator.Key, defaultValue);

        public void SetValue<T>(string key, T value) => keyValues[key] = value;
        public T GetValue<T>(string key) => GetValue<T>(key, default);
        public T GetValue<T>(string key, T defaultValue)
        {
            if (!keyValues.ContainsKey(key))
                SetValue(key, defaultValue);

            return (T)keyValues[key];
        }

        public void Write()
        {
            if (IsErrorOccured)
                return;

            SettingsStore.Write(keyValues);
        }

        public void Read() => keyValues = SettingsStore.Read();
        public void ErrorOccurred() => IsErrorOccured = true;
    }
}
