using Common.SettingBackupAndRestore;
using ImageProcess.Buffer;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeImageBufferBGRA32
{
    internal class ImageBufferBGRA32DataSelectorDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<bool?> BlockItemDataInputDC { get; }
        public BlockItemDataInputDC<ImageBufferBGRA32> BlockItemDataInputDC_A { get; }
        public BlockItemDataInputDC<ImageBufferBGRA32> BlockItemDataInputDC_B { get; }
        public BlockItemDataOutputDC<ImageBufferBGRA32> BlockItemDataOutputDC { get; }


        public ImageBufferBGRA32DataSelectorDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC = new BlockItemDataInputDC<bool?>(dependencyParams);
            BlockItemDataInputDC_A = new BlockItemDataInputDC<ImageBufferBGRA32>(dependencyParams);
            BlockItemDataInputDC_B = new BlockItemDataInputDC<ImageBufferBGRA32>(dependencyParams);

            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<ImageBufferBGRA32>();
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
            bool? value = BlockItemDataInputDC.Value;

            if (!value.HasValue)
            {
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
                BlockItemDataOutputDC.Value = null;
                return;
            }

            if (value.Value)
            {
                ImageBufferBGRA32 value_A = BlockItemDataInputDC_A.Value;

                if (value_A == null)
                    SetStatus(Status.Error, "Az 'A' bemenetre nem érkezett kiértékelhető eredmény!");
                else
                    SetStatus(Status.Ok);

                BlockItemDataOutputDC.Value = value_A;
            }
            else
            {
                ImageBufferBGRA32 value_B = BlockItemDataInputDC_B.Value;

                if (value_B == null)
                    SetStatus(Status.Error, "Az 'B' bemenetre nem érkezett kiértékelhető eredmény!");
                else
                    SetStatus(Status.Ok);

                BlockItemDataOutputDC.Value = value_B;
            }
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
