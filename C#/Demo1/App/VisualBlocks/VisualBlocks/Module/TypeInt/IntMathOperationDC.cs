using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeInt
{
    internal class IntMathOperationDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_A { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC_B { get; }
        public BlockItemDataOutputDC<int?> BlockItemDataOutputDC { get; }

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


        public IntMathOperationDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_A = new BlockItemDataInputDC<int?>(dependencyParams);
            BlockItemDataInputDC_B = new BlockItemDataInputDC<int?>(dependencyParams);
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
            int? value_A = BlockItemDataInputDC_A.Value;
            int? value_B = BlockItemDataInputDC_B.Value;

            if (!value_A.HasValue && !value_B.HasValue)
            {
                SetStatus(Status.Error, "Az 'A' és 'B' bemenetekre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (!value_A.HasValue)
            {
                SetStatus(Status.Error, "Az 'A' bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (!value_B.HasValue)
            {
                SetStatus(Status.Error, "A 'B' bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            try
            {
                switch (selectedOperationIndex)
                {
                    case 0:
                        BlockItemDataOutputDC.Value = Convert.ToInt32(value_A + value_B);
                        break;

                    case 1:
                        BlockItemDataOutputDC.Value = Convert.ToInt32(value_A - value_B);
                        break;

                    case 2:
                        BlockItemDataOutputDC.Value = Convert.ToInt32(value_A * value_B);
                        break;

                    case 3:
                        BlockItemDataOutputDC.Value = Convert.ToInt32(value_A / value_B);
                        break;

                    case 4:
                        if (!value_A.HasValue || !value_B.HasValue)
                            BlockItemDataOutputDC.Value = null;
                        else
                            BlockItemDataOutputDC.Value = Convert.ToInt32(Math.Pow(value_A.Value, value_B.Value));
                        break;
                }

                SetStatus(Status.Ok);
            }
            catch
            {
                SetStatus(Status.Error, $"A kiszámolt érték nincs a minimum és maximum értékek között!\nMinimum: {Int32.MinValue}\nMaximum: {Int32.MaxValue}");
                BlockItemDataOutputDC.Value = null;
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
