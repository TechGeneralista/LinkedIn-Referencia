using System.Collections.Generic;


namespace Common.Settings
{
    public interface ISettingsStore
    {
        void Write(Dictionary<string, object> dictionary);
        Dictionary<string, object> Read();
    }
}