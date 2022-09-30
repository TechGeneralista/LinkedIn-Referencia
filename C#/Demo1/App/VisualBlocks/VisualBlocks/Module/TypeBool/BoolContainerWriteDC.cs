using Common.SettingBackupAndRestore;
using Common.Types;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeBool
{
    internal class BoolContainerWriteDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<Container<bool?>> BlockItemDataInputDC_Container { get; }
        public BlockItemDataInputDC<bool?> BlockItemDataInputDC_Data { get; }


        public BoolContainerWriteDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC_Container = new BlockItemDataInputDC<Container<bool?>>(dependencyParams);
            BlockItemDataInputDC_Data = new BlockItemDataInputDC<bool?>(dependencyParams);

            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemTriggerOutputDC = new BlockItemTriggerOutputDC(dependencyParams);
        }

        private void BlockItemTriggerInputDC_TriggerAction(object sender, TriggerActionArgs e)
        {
            ExecuteOperation();
            BlockItemTriggerOutputDC.Trigger(e.Token);
        }

        private void BlockItemDataOutputDC_PullAction(object sender, EventArgs e)
        {
            // Húzásra csak akkor indítjuk a műveletet ha a trigger nincs csatlakoztatva
            if (!BlockItemTriggerInputDC.Connected)
                ExecuteOperation();
        }

        private void ExecuteOperation()
        {
            Container<bool?> container = BlockItemDataInputDC_Container.Value;

            if (container == null)
            {
                SetStatus(Status.Error, "A 'tároló' bemenetre nem érkezett kiértékelhető eredmény!");
                return;
            }

            bool? value = BlockItemDataInputDC_Data.Value;

            if (!value.HasValue)
            {
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
                return;
            }

            container.Value = value;
            SetStatus(Status.Ok);
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
