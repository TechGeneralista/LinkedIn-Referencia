using System.Collections.Generic;
using System.Linq;


namespace Common.Settings
{
    public class SettingsCollection : ISettingsCollection
    {
        public int Count => keyValues.Keys.Count;
        public string[] Keys => keyValues.Keys.ToArray();


        readonly ISettingsStore settingsStore;
        Dictionary<string, object> keyValues = new Dictionary<string, object>();


        public SettingsCollection(ISettingsStore settingsStore)
        {
            this.settingsStore = settingsStore;
            keyValues = settingsStore.Read();
        }


        public void SetValue<T>(string key, T value) => keyValues[key] = value;

        public T GetValue<T>(string key) => GetValue<T>(key, default);

        public T GetValue<T>(string key, T defaultValue)
        {
            if (!keyValues.ContainsKey(key))
                SetValue(key, defaultValue);

            return (T)keyValues[key];
        }

        public void Write() => settingsStore.Write(keyValues);
    }
}
