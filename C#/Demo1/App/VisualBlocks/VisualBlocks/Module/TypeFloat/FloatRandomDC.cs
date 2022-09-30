using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeFloat
{
    internal class FloatRandomDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<float?> BlockItemDataInputDC_Min { get; }
        public BlockItemDataInputDC<float?> BlockItemDataInputDC_Max { get; }
        public BlockItemDataOutputDC<float?> BlockItemDataOutputDC { get; }


        Random random;


        public FloatRandomDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_Min = new BlockItemDataInputDC<float?>(dependencyParams);
            BlockItemDataInputDC_Max = new BlockItemDataInputDC<float?>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<float?>();
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
            float? value_Min = BlockItemDataInputDC_Min.Value;
            float? value_Max = BlockItemDataInputDC_Max.Value;

            if (!value_Min.HasValue && !value_Max.HasValue)
                SetStatus(Status.Error, "Az 'A' és 'B' bemenetekre nem érkezett kiértékelhető eredmény!");
            else if (!value_Min.HasValue)
                SetStatus(Status.Error, "Az 'A' bemenetre nem érkezett kiértékelhető eredmény!");
            else if (!value_Max.HasValue)
                SetStatus(Status.Error, "A 'B' bemenetre nem érkezett kiértékelhető eredmény!");
            else if (value_Min > value_Max)
                SetStatus(Status.Error, "A 'min' nem lehet nagyobb mint a 'max'!");
            else
                SetStatus(Status.Ok);

            BlockItemDataOutputDC.Value = Convert.ToSingle(value_Min + (random.NextDouble() * (value_Max - value_Min)));
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
