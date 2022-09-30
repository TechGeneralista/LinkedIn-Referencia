using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeInt
{
    internal class IntMonitorDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC { get; }
        public BlockItemDataOutputDC<int?> BlockItemDataOutputDC { get; }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            { 
                SetField(ref isEnabled, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        bool isEnabled = true;

        public string Value
        {
            get => value;
            private set => SetField(ref this.value, value);
        }
        string value;


        public IntMonitorDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC = new BlockItemDataInputDC<int?>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<int?>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;

            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
        {
            ExecuteOperation();
            BlockItemTriggerOutputDC.Trigger(e.Token);
            BlockItemDataOutputDC.Value = null;
        }

        private void BlockItemDataOutputDC_PullAction(object sender, EventArgs e)
        {
            // Húzásra csak akkor indítjuk a műveletet ha a trigger nincs csatlakoztatva
            if (!BlockItemTriggerInputDC.Connected)
                ExecuteOperation();
        }

        private void ExecuteOperation()
        {
            int? value = BlockItemDataInputDC.Value;

            if (!value.HasValue)
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
            else
                SetStatus(Status.Ok);

            if (isEnabled && value.HasValue)
                Value = value.ToString();
            else
                Value = string.Empty;

            BlockItemDataOutputDC.Value = value;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(IsEnabled), IsEnabled);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            IsEnabled = bar.GetData<bool>(nameof(IsEnabled));
        }
    }
}
