using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeInt
{
    internal class IntRandomDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_Min { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_Max { get; }
        public BlockItemDataOutputDC<int?> BlockItemDataOutputDC { get; }


        Random random;


        public IntRandomDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_Min = new BlockItemDataInputDC<int?>(dependencyParams);
            BlockItemDataInputDC_Max = new BlockItemDataInputDC<int?>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<int?>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;

            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            random = new Random();
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
            int? value_Min = BlockItemDataInputDC_Min.Value;
            int? value_Max = BlockItemDataInputDC_Max.Value;

            if (!value_Min.HasValue && !value_Max.HasValue)
            {
                SetStatus(Status.Error, "Az 'A' és 'B' bemenetekre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (!value_Min.HasValue)
            {
                SetStatus(Status.Error, "Az 'A' bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (!value_Max.HasValue)
            {
                SetStatus(Status.Error, "A 'B' bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if(value_Min > value_Max)
            {
                SetStatus(Status.Error, "A 'min' nem lehet nagyobb mint a 'max'!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            SetStatus(Status.Ok);
            BlockItemDataOutputDC.Value = Convert.ToInt32(random.Next(value_Min.Value, value_Max.Value));
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
    }
}
