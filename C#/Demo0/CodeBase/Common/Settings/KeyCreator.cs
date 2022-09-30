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
                CreateKey();
                return key;
            }
        }


        List<string> keyList = new List<string>();
        bool changed;
        string key;


        public void AddNew(string key)
        {
            changed = true;
            keyList.Add(key);
        }

        public void ReplaceLast(string key)
        {
            changed = true;
            keyList[keyList.Count - 1] = key;
        }

        public void RemoveLast(int count = 1)
        {
            changed = true;

            for(int i=0;i<count;i++)
                keyList.RemoveAt(keyList.Count - 1);
        }

        public void RemoveAll()
        {
            changed = true;
            keyList.Clear();
        }

        private void CreateKey()
        {
            if (!changed)
                return;

            StringBuilder stringBuilder = new StringBuilder();

            foreach(string s in keyList)
            {
                stringBuilder.Append(s);

                if (s != keyList.Last())
                    stringBuilder.Append('/');
            }

            key = stringBuilder.ToString();

            changed = false;
        }
    }
}