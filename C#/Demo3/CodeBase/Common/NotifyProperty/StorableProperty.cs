using Common.Settings;
using System;
using System.Linq;
using System.Text;


namespace Common.NotifyProperty
{
    public class StorableProperty<T> : Property<T>
    {
        public StorableProperty(ISettingsCollection settingsCollection, params string[] keys) 
            : this(default, null, settingsCollection, keys) { }
        
        public StorableProperty(T initialValue, ISettingsCollection settingsCollection, params string[] keys) 
            : this(initialValue, null, settingsCollection, keys) { }
       
        public StorableProperty(Action<T, T> currentValueChanged, ISettingsCollection settingsCollection, params string[] keys) 
            : this(default, currentValueChanged, settingsCollection, keys) { }

        public StorableProperty(T initialValue, Action<T, T> currentValueChanged, ISettingsCollection settingsCollection, params string[] keys)
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
                OnValueChanged += currentValueChanged;

            if (settingsCollection.IsNotNull())
            {
                Value = settingsCollection.GetValue(key, initialValue);
                OnValueChanged += (o, n) => settingsCollection.SetValue(key, n);
            }
            else
                Value = initialValue;
        }
    }
}
