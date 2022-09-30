using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common.Settings
{
    public class KeyCreator
    {
        public string Key
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (string s in keyList)
                {
                    stringBuilder.Append(s);
                    stringBuilder.Append('/');
                }

                stringBuilder.Remove(stringBuilder.Length-1, 1);
                return stringBuilder.ToString();
            }
        }

        public string[] Keys => keyList.ToArray();


        readonly List<string> keyList = new List<string>();
        readonly Stack<int> keyStack = new Stack<int>();


        public void AddNew(params string[] keys)
            => keyList.AddRange(keys);

        public void ReplaceLast(string key)
            => keyList[keyList.Count - 1] = key;

        public void RemoveLast(int count = 1)
        {
            for (int i = 0; i < count; i++)
                keyList.RemoveAt(keyList.Count - 1);
        }

        public void RemoveAll()
            => keyList.Clear();

        public void ObjectEntryPoint(string name)
        {
            keyStack.Push(keyList.Count);
            AddNew(name);
        }

        public void ObjectExitPoint()
        {
            int resetCount = keyStack.Pop();
            RemoveLast(keyList.Count - resetCount);
        }
    }
}