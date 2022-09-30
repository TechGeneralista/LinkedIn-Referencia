using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Common.Tool
{


    public class ReadableConfigFile
    {
        public Exception Error { get; private set; }


        readonly string filePath;
        readonly char separator;
        Dictionary<string, string> dict;


        public ReadableConfigFile(string filePath, char separator)
        {
            this.filePath = filePath;
            this.separator = separator;
        }

        public void ReadFromDisk()
        {
            dict = new Dictionary<string, string>();

            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                foreach(string line in lines)
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] keyVal = line.Split(separator);

                    if (keyVal.Length == 2)
                        dict[keyVal[0]] = keyVal[1];
                }

                Error = null;
            }

            catch(Exception ex)
            {
                Error = ex;
            }
        }

        public string GetValue(string key)
        {
            if (!dict.ContainsKey(key))
                return string.Empty;

            return dict[key];
        }
    }
}
