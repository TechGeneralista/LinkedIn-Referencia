using System.Collections.Generic;
using System.IO;


namespace Common.Settings
{
    public class FileSettingsStore : ISettingsStore
    {
        public bool IsErrorOccured { get; private set; }
        public string FileName { get; private set; }


        public FileSettingsStore(string fileName)
        {
            FileName = fileName;
        }


        public Dictionary<string, object> Read()
        {
            Dictionary<string, object> dictionary = null;

            try
            {
                dictionary = File.ReadAllBytes(FileName).Deserialize<Dictionary<string, object>>();
            }
            catch
            {
                dictionary = new Dictionary<string, object>();
                IsErrorOccured = true;
            }

            return dictionary;
        }

        public void Write(Dictionary<string, object> dictionary)
        {
            try
            {
                string dirName = Path.GetDirectoryName(FileName);
                Utils.CreateDirectoryIfNotExists(dirName);
                File.WriteAllBytes(FileName, dictionary.Serialize());
            }
            catch
            {
                IsErrorOccured = true;
            }
        }
    }
}
