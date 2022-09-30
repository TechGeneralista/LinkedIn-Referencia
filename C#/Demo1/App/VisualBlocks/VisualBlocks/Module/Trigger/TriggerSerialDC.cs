using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Threading;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.Trigger
{
    internal class TriggerSerialDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_A { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC_B { get; }


        public TriggerSerialDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemTriggerOutputDC_A = new BlockItemTriggerOutputDC(dependencyParams);
            BlockItemTriggerOutputDC_B = new BlockItemTriggerOutputDC(dependencyParams);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
        {
            BlockItemTriggerOutputDC_A.Trigger(e.Token);
            BlockItemTriggerOutputDC_B.Trigger(e.Token);
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
