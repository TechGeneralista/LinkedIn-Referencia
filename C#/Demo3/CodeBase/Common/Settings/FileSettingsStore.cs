using System.Collections.Generic;
using System.IO;


namespace Common.Settings
{
    public class FileSettingsStore : ISettingsStore
    {
        public bool IsErrorOccured { get; private set; }
        public string FilePath { get; private set; }


        public FileSettingsStore(string filePath)
        {
            FilePath = filePath;
        }


        public Dictionary<string, object> Read()
        {
            Dictionary<string, object> dictionary = null;

            try
            {
                dictionary = File.ReadAllBytes(FilePath).Deserialize<Dictionary<string, object>>();
                IsErrorOccured = false;
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
                string dirName = Path.GetDirectoryName(FilePath);
                Utils.CreateDirectoryIfNotExists(dirName);
                File.WriteAllBytes(FilePath, dictionary.Serialize());
                IsErrorOccured = false;
            }
            catch
            {
                IsErrorOccured = true;
            }
        }
    }
}
