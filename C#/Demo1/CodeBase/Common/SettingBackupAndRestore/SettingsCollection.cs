using System.Collections.Generic;


namespace Common.SettingBackupAndRestore
{
    public class SettingsCollection
    {
        public Dictionary<string, object> Container { get; }


        public SettingsCollection(Dictionary<string, object> container = null)
        {
            if(container != null)
                Container = container;
            else
                Container = new Dictionary<string, object>();
        }

        public void SetValue<T>(string key, T value) => Container[key] = value;
        public T GetValue<T>(string key) => GetValue<T>(key, default);
        public T GetValue<T>(string key, T defaultValue)
        {
            if (!Container.ContainsKey(key))
                SetValue(key, defaultValue);

            return (T)Container[key];
        }
    }
}
