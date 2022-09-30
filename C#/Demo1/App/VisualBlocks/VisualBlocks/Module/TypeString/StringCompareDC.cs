using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeString
{
    internal class StringCompareDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<string> BlockItemDataInputDC_A { get; }
        public BlockItemDataInputDC<string> BlockItemDataInputDC_B { get; }
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
        int selectedOperationIndex = 0;


        public StringCompareDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_A = new BlockItemDataInputDC<string>(dependencyParams);
            BlockItemDataInputDC_B = new BlockItemDataInputDC<string>(dependencyParams);
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
            string value_A = BlockItemDataInputDC_A.Value;
            string value_B = BlockItemDataInputDC_B.Value;

            if (value_A == null && value_B == null)
            {
                SetStatus(Status.Error, "Az 'A' és 'B' bemenetekre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (value_A == null)
            {
                SetStatus(Status.Error, "Az 'A' bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (value_B == null)
            {
                SetStatus(Status.Error, "A 'B' bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            SetStatus(Status.Ok);

            switch(selectedOperationIndex)
            {
                case 0:
                    BlockItemDataOutputDC.Value = value_A == value_B;
                    break;

                case 1:
                    BlockItemDataOutputDC.Value = value_A.Contains(value_B);
                    break;

                case 2:
                    BlockItemDataOutputDC.Value = value_A.StartsWith(value_B);
                    break;

                case 3:
                    BlockItemDataOutputDC.Value = value_A.EndsWith(value_B);
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
