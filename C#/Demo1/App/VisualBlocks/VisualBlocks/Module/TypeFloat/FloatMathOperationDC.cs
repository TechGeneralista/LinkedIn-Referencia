using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeFloat
{
    internal class FloatMathOperationDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<float?> BlockItemDataInputDC_A { get; }
        public BlockItemDataInputDC<float?> BlockItemDataInputDC_B { get; }
        public BlockItemDataOutputDC<float?> BlockItemDataOutputDC { get; }

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


        public FloatMathOperationDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_A = new BlockItemDataInputDC<float?>(dependencyParams);
            BlockItemDataInputDC_B = new BlockItemDataInputDC<float?>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<float?>();
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
            float? value_A = BlockItemDataInputDC_A.Value;
            float? value_B = BlockItemDataInputDC_B.Value;

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
                        BlockItemDataOutputDC.Value = Convert.ToSingle(value_A + value_B);
                        break;

                    case 1:
                        BlockItemDataOutputDC.Value = Convert.ToSingle(value_A - value_B);
                        break;

                    case 2:
                        BlockItemDataOutputDC.Value = Convert.ToSingle(value_A * value_B);
                        break;

                    case 3:
                        BlockItemDataOutputDC.Value = Convert.ToSingle(value_A / value_B);
                        break;

                    case 4:
                        if (!value_A.HasValue || !value_B.HasValue)
                            BlockItemDataOutputDC.Value = null;
                        else
                            BlockItemDataOutputDC.Value = Convert.ToSingle(Math.Pow(value_A.Value, value_B.Value));
                        break;
                }

                SetStatus(Status.Ok);
            }
            catch
            {
                SetStatus(Status.Error, $"A kiszámolt érték nincs a minimum és maximum értékek között!\nMinimum: {float.MinValue}\nMaximum: {float.MaxValue}");
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
