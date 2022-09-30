using Common.SettingBackupAndRestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.Trigger
{
    internal class TriggerInitializeDC : BlockItemDC, ICanBackupAndRestore, IInitializable
    {
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }


        Task task;


        public TriggerInitializeDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);
        }

        internal async void Trigger()
        {
            if (task == null || (task != null && task.Status == TaskStatus.RanToCompletion))
                await (task = Task.Run(() => BlockItemTriggerOutputDC.Trigger()));
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
        }

        public void Initialize()
            => Trigger();
    }
}
