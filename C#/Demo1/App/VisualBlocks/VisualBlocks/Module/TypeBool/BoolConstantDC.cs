using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeBool
{
    internal class BoolConstantDC : BlockItemDC, ICanBackupAndRestore
    {
        public bool LogicalValue
        {
            get => logicalValue;

            set
            {
                SetField(ref logicalValue, value);
                dependencyParams.NotifyProjectChanged();
                ExecuteOperationAndTrigger();
            }
        }
        bool logicalValue;

        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataOutputDC<bool?> BlockItemDataOutputDC { get; }


        Task task;


        public BoolConstantDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            BlockItemDataOutputDC = new BlockItemDataOutputDC<bool?>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;
        }

        private void BlockItemDataOutputDC_PullAction(object sender, EventArgs e)
            => ExecuteOperation();

        private async void ExecuteOperationAndTrigger()
        {
            ExecuteOperation();

            if (task == null || (task != null && task.Status == TaskStatus.RanToCompletion))
                await (task = Task.Run(() => BlockItemTriggerOutputDC.Trigger()));
        }

        private void ExecuteOperation()
        {
            SetStatus(Status.Ok);
            BlockItemDataOutputDC.Value = logicalValue;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(LogicalValue), LogicalValue);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            logicalValue = bar.GetData<bool>(nameof(LogicalValue));
        }
    }
}
