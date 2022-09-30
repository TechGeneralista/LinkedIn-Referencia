using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeFloat
{
    internal class FloatConstantDC : BlockItemDC, ICanBackupAndRestore
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
        public BlockItemDataOutputDC<float?> BlockItemDataOutputDC { get; }


        Task task;


        public FloatConstantDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            BlockItemDataOutputDC = new BlockItemDataOutputDC<float?>();
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
            if(string.IsNullOrEmpty(typedValue) || string.IsNullOrWhiteSpace(typedValue))
                SetStatus(Status.Error, "Nincs megadott érték!");
            else if(float.TryParse(typedValue, out float f))
            {
                SetStatus(Status.Ok);
                BlockItemDataOutputDC.Value = f;
                return;
            }
            else
                SetStatus(Status.Error, $"A megadott érték nem értelmezhető számként\nvagy nincs a minimum és maximum értékek között\nvagy nem megfelelő tizedes elválasztó karakter '.' <=> ','\nMinimum: {float.MinValue}\nMaximum: {float.MaxValue}");

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
