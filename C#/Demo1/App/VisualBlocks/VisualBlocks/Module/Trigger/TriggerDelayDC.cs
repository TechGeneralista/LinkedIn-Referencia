using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Threading;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.Trigger
{
    internal class TriggerDelayDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<int?> BlockItemDataInputDC { get; }


        public TriggerDelayDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);

            BlockItemDataInputDC = new BlockItemDataInputDC<int?>(dependencyParams);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
            => ExecuteOperation(e);

        private void ExecuteOperation(TriggerActionArgs e)
        {
            int? value = BlockItemDataInputDC.Value;

            if (!value.HasValue)
            {
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
                return;
            }

            if(value.Value < 0)
            {
                SetStatus(Status.Error, "A 'ms' értéknek nagyobbnak vagy egyenlőnek kell lennie nullánál!");
                return;
            }

            SetStatus(Status.Ok);

            if(value != 0)
                Thread.Sleep(value.Value);
            
            BlockItemTriggerOutputDC.Trigger(e.Token);
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
