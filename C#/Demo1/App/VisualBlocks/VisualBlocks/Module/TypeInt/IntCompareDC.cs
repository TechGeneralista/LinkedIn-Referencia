using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeInt
{
    internal class IntCompareDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_A { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_B { get; }
        public BlockItemDataOutputDC<bool?> BlockItemDataOutputDC { get; }

        public int SelectedOperationIndex
        {
            get => selectedOperationIndex;
            set
            {
                SetField(ref selectedOperationIndex, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        int selectedOperationIndex = 2;


        public IntCompareDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_A = new BlockItemDataInputDC<int?>(dependencyParams);
            BlockItemDataInputDC_B = new BlockItemDataInputDC<int?>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<bool?>();
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
            int? value_A = BlockItemDataInputDC_A.Value;
            int? value_B = BlockItemDataInputDC_B.Value;

            if (!value_A.HasValue && !value_B.HasValue)
                SetStatus(Status.Error, "Az 'A' és 'B' bemenetekre nem érkezett kiértékelhető eredmény!");
            else if (!value_A.HasValue)
                SetStatus(Status.Error, "Az 'A' bemenetre nem érkezett kiértékelhető eredmény!");
            else if (!value_B.HasValue)
                SetStatus(Status.Error, "A 'B' bemenetre nem érkezett kiértékelhető eredmény!");
            else
                SetStatus(Status.Ok);

            switch(selectedOperationIndex)
            {
                case 0:
                    BlockItemDataOutputDC.Value = value_A > value_B;
                    break;

                case 1:
                    BlockItemDataOutputDC.Value = value_A >= value_B;
                    break;

                case 2:
                    BlockItemDataOutputDC.Value = value_A == value_B;
                    break;

                case 3:
                    BlockItemDataOutputDC.Value = value_A <= value_B;
                    break;

                case 4:
                    BlockItemDataOutputDC.Value = value_A < value_B;
                    break;
            }
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(SelectedOperationIndex), SelectedOperationIndex);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            SelectedOperationIndex = bar.GetData<int>(nameof(SelectedOperationIndex));
        }
    }
}
