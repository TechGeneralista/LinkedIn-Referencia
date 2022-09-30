using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common.SettingBackupAndRestore
{
    public class KeySettingsCollection : SettingsCollection
    {
        Stack<string> keyStack = new Stack<string>();

        public KeySettingsCollection(Dictionary<string, object> container = null) : base(container) { }

        public void AddNewKey(string key)
            => keyStack.Push(key);

        public void RemoveLastKey(int i = 1)
        {
            while(i-- != 0)
                keyStack.Pop();
        }

        public void ReplaceLastKey(string key)
        {
            keyStack.Pop();
            AddNewKey(key);
        }

        public void SetData<T>(string key, T value)
        {
            AddNewKey(key);
            SetValue(GetKey(), value);
            RemoveLastKey();
        }

        public T GetData<T>(string key, T defaultValue = default(T))
        {
            AddNewKey(key);
            T value = GetValue(GetKey(), defaultValue);
            RemoveLastKey();
            return value;
        }

        private string GetKey()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for(int i=keyStack.Count-1;i != -1;i -= 1)
            {
                stringBuilder.Append(keyStack.ElementAt(i));
                stringBuilder.Append('/');
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
    }
}
