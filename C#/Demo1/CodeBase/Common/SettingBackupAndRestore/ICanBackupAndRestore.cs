using System.Collections.Generic;


namespace Common.SettingBackupAndRestore
{
    public interface ICanBackupAndRestore
    {
        Dictionary<string, object> Backup();
        void Restore(Dictionary<string, object> container);
    }
}
