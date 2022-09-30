using System.Collections.Generic;


namespace Common.Settings
{
    public interface ISettingsStore
    {
        bool IsErrorOccured { get; }


        void Write(Dictionary<string, object> dictionary);
        Dictionary<string, object> Read();
    }
}