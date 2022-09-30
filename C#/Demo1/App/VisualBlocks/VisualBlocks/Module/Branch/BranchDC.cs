using Common.SettingBackupAndRestore;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.Branch
{
    internal class BranchDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_True { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_False { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_Then { get; }
        public BlockItemDataInputDC<bool?> BlockItemDataInputDC { get; }


        public BranchDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemTriggerOutputDC_True = new BlockItemTriggerOutputDC(dependencyParams);
            BlockItemTriggerOutputDC_False = new BlockItemTriggerOutputDC(dependencyParams);
            BlockItemTriggerOutputDC_Then = new BlockItemTriggerOutputDC(dependencyParams);

            BlockItemDataInputDC = new BlockItemDataInputDC<bool?>(dependencyParams);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
            => ExecuteOperation(e);

        private void ExecuteOperation(TriggerActionArgs e)
        {
            object value = BlockItemDataInputDC.Value;

            if (value == null)
            {
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
                return;
            }

            SetStatus(Status.Ok);

            if ((bool)value)
                BlockItemTriggerOutputDC_True.Trigger(e.Token);
            else
                BlockItemTriggerOutputDC_False.Trigger(e.Token);

            BlockItemTriggerOutputDC_Then.Trigger(e.Token);
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
