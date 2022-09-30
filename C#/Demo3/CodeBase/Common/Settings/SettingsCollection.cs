using System.Collections.Generic;


namespace Common.Settings
{
    public class SettingsCollection : ISettingsCollection
    {
        public ISettingsStore SettingsStore { get; }
        public bool IsErrorOccured { get; private set; }


        Dictionary<string, object> keyValues = new Dictionary<string, object>();
        readonly KeyCreator keyCreator = new KeyCreator();


        public SettingsCollection(ISettingsStore settingsStore, bool read = false)
        {
            SettingsStore = settingsStore;

            if (read)
                Read();
        }

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

        public string GetNextKey()
        {
            string[] currentKeys = keyCreator.Keys;

            foreach(string s in keyValues.Keys)
            {
                string[] skeys = s.Split('/');

                if (skeys.Compare(currentKeys))
                    return skeys[currentKeys.Length];
            }

            throw new KeyNotFoundException(keyCreator.Key);
        }

        public void EntryPoint(string name) => keyCreator.ObjectEntryPoint(name);
        public void ExitPoint() => keyCreator.ObjectExitPoint();

        public void SetProperty<T>(T propertyValue, params string[] propertyNames)
        {
            keyCreator.AddNew(propertyNames);
            SetValue(keyCreator.Key, propertyValue);
            keyCreator.RemoveLast(propertyNames.Length);
        }

        public T GetProperty<T>(params string[] propertyNames)
        {
            keyCreator.AddNew(propertyNames);
            T propertyValue = GetValue<T>(keyCreator.Key);
            keyCreator.RemoveLast(propertyNames.Length);
            return propertyValue;
        }

        public void AddKey(params string[] keys)
            => keyCreator.AddNew(keys);

        public void RemoveLastKey(int count = 1)
            => keyCreator.RemoveLast(count);

        public void ReplaceLastKey(string key)
            => keyCreator.ReplaceLast(key);
    }
}
