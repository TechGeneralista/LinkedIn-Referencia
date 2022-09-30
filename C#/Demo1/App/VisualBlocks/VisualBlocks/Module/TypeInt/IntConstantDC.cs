using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeInt
{
    internal class IntConstantDC : BlockItemDC, ICanBackupAndRestore
    {
        public string TypedValue
        {
            get => typedValue;

            set
            {
                SetField(ref typedValue, value);
                dependencyParams.NotifyProjectChanged();
                ExecuteOperationAndTrigger();
            }
        }
        string typedValue;

        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataOutputDC<int?> BlockItemDataOutputDC { get; }


        Task task;


        public IntConstantDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            BlockItemDataOutputDC = new BlockItemDataOutputDC<int?>();
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
            if (string.IsNullOrEmpty(typedValue) || string.IsNullOrWhiteSpace(typedValue))
                SetStatus(Status.Error, "Nincs megadott érték!");
            else if (int.TryParse(typedValue, out int i))
            {
                SetStatus(Status.Ok);
                BlockItemDataOutputDC.Value = i;
                return;
            }
            else
                SetStatus(Status.Error, $"A megadott érték nem értelmezhető számként vagy nincs a minimum és maximum értékek között!\nMinimum: {Int32.MinValue}\nMaximum: {Int32.MaxValue}");

            BlockItemDataOutputDC.Value = null;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(TypedValue), TypedValue);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            typedValue = bar.GetData<string>(nameof(TypedValue));
        }
    }
}
