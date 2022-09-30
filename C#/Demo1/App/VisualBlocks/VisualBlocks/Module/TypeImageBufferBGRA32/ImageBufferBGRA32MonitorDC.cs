using Common.SettingBackupAndRestore;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using VisualBlocks.Module.Base;
using ipb = ImageProcess.Buffer;


namespace VisualBlocks.Module.TypeImageBufferBGRA32
{
    internal class ImageBufferBGRA32MonitorDC : BlockItemDC, ICanBackupAndRestore
    {
        public BlockItemTriggerInputDC BlockItemTriggerInputDC { get; }
        public BlockItemTriggerOutputDC BlockItemTriggerOutputDC { get; }
        public BlockItemDataInputDC<ipb.ImageBufferBGRA32> BlockItemDataInputDC { get; }
        public BlockItemDataOutputDC<ipb.ImageBufferBGRA32> BlockItemDataOutputDC { get; }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                SetField(ref isEnabled, value);
                dependencyParams.NotifyProjectChanged();
            }
        }
        bool isEnabled = true;

        public BitmapSource ImageSource
        {
            get => imageSource;
            private set => SetField(ref imageSource, value);
        }
        BitmapSource imageSource;


        public ImageBufferBGRA32MonitorDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            BlockItemDataInputDC = new BlockItemDataInputDC<ipb.ImageBufferBGRA32>(dependencyParams);
            BlockItemTriggerInputDC = new BlockItemTriggerInputDC(dependencyParams);
            BlockItemTriggerInputDC.TriggerAction += BlockItemTriggerInputDC_TriggerAction;

            BlockItemDataOutputDC = new BlockItemDataOutputDC<ipb.ImageBufferBGRA32>();
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
            ipb.ImageBufferBGRA32 value = BlockItemDataInputDC.Value;

            if(value == null)
                SetStatus(Status.Error, "A bemenetre nem érkezett kiértékelhető eredmény!");
            else
                SetStatus(Status.Ok);

            if (isEnabled && value is ipb.ImageBufferBGRA32 imageBuffer)
                ImageSource = imageBuffer.Upload();

            BlockItemDataOutputDC.Value = value;
        }

        public Dictionary<string, object> Backup()
        {
            BackupAndRestore bar = new BackupAndRestore();
            bar.SetData(nameof(Left), Left);
            bar.SetData(nameof(Top), Top);
            bar.SetData(nameof(IsEnabled), IsEnabled);
            return bar.Container;
        }

        public void Restore(Dictionary<string, object> container)
        {
            BackupAndRestore bar = new BackupAndRestore(container);
            Left = bar.GetData<double>(nameof(Left));
            Top = bar.GetData<double>(nameof(Top));
            IsEnabled = bar.GetData<bool>(nameof(IsEnabled));
        }
    }
}
