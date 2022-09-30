using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Common.SettingBackupAndRestore
{
    public delegate object CreateInstanceDelegate(Type type);

    public class BackupAndRestore : KeySettingsCollection
    {
        readonly string dataContainerKey = "DataContainer";

        public BackupAndRestore(Dictionary<string, object> container = null) : base(container) { }

        public void SetList(string name, IList list)
        {
            AddNewKey(name);
            SetData(nameof(IList.Count), list.Count);
            AddNewKey("temp");

            foreach (object obj in list)
            {
                int index = list.IndexOf(obj);
                ReplaceLastKey(index.ToString());

                string typeAssemblyQualifiedName = obj.GetType().AssemblyQualifiedName;
                SetData(nameof(Type) + nameof(Type.AssemblyQualifiedName), typeAssemblyQualifiedName);

                if (obj is ICanBackupAndRestore canBackupAndRestore)
                    SetData(dataContainerKey, canBackupAndRestore.Backup());
            }

            RemoveLastKey(2);
        }

        public void GetList(string name, IList list, CreateInstanceDelegate createInstanceMethod)
        {
            int error = GetListInternal(name, list, createInstanceMethod);

            if (error != 0)
                MessageBox.Show($"Nem sikerült minden elemet visszaállítani! {error}", "Figyelmeztetés:", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private int GetListInternal(string name, IList list, CreateInstanceDelegate createInstanceMethod)
        {
            int error = 0;

            AddNewKey(name);
            int count = GetData<int>(nameof(IList.Count));
            AddNewKey("temp");

            for (int index = 0; index < count; index += 1)
            {
                ReplaceLastKey(index.ToString());
                string typeAssemblyQualifiedName = GetData<string>(nameof(Type) + nameof(Type.AssemblyQualifiedName));
                Type restoredType = Type.GetType(typeAssemblyQualifiedName);

                if (restoredType == null)
                {
                    error += 1;
                    Trace.WriteLine($"{error} A típus nem támogatott: {typeAssemblyQualifiedName}");
                    continue;
                }

                object obj = createInstanceMethod(restoredType);

                if (obj == null)
                {
                    error += 1;
                    Trace.WriteLine($"{error} A típust nem sikerült példányosítani: {restoredType.AssemblyQualifiedName}");
                    continue;
                }

                try
                {
                    if (obj is ICanBackupAndRestore canBackupAndRestore)
                        canBackupAndRestore.Restore(GetData<Dictionary<string, object>>(dataContainerKey));
                }
                catch
                {
                    error += 1;
                    Trace.WriteLine($"{error} A típust nem sikerült visszaállítani: {restoredType.AssemblyQualifiedName}");
                    continue;
                }

                list.Add(obj);
            }

            RemoveLastKey(2);
            return error;
        }
    }
}
