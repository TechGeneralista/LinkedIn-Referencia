using Common.Settings;
using System;
using System.Linq;
using System.Text;


namespace Common.NotifyProperty
{
    public class StorableObservableProperty<T> : ObservableProperty<T>
    {
        public StorableObservableProperty(ISettingsCollection settingsCollection, params string[] keys) 
            : this(default, null, settingsCollection, keys) { }
        
        public StorableObservableProperty(T initialValue, ISettingsCollection settingsCollection, params string[] keys) 
            : this(initialValue, null, settingsCollection, keys) { }
       
        public StorableObservableProperty(Action<T, T> currentValueChanged, ISettingsCollection settingsCollection, params string[] keys) 
            : this(default, currentValueChanged, settingsCollection, keys) { }

        public StorableObservableProperty(T initialValue, Action<T, T> currentValueChanged, ISettingsCollection settingsCollection, params string[] keys)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (string s in keys)
            {
                stringBuilder.Append(s);

                if (s != keys.Last())
                    stringBuilder.Append('/');
            }

            string key = stringBuilder.ToString();

            if (currentValueChanged.IsNotNull())
                CurrentValueChanged += currentValueChanged;

            CurrentValue = settingsCollection.GetValue(key, initialValue);
            CurrentValueChanged += (o, n) => settingsCollection.SetValue(key, n);
        }
    }
}
