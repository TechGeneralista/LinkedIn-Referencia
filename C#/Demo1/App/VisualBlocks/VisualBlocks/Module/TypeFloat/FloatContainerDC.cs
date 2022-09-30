using Common.SettingBackupAndRestore;
using Common.Types;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeFloat
{
    internal class FloatContainerDC : BlockItemDC, ICanBackupAndRestore
    {
        public string CurrentValue
        {
            get => currentValue;
            private set => SetField(ref currentValue, value);
        }
        private string currentValue;

        public BlockItemDataOutputDC<Container<float?>> BlockItemDataOutputDC { get; }


        readonly Container<float?> container;


        public FloatContainerDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            container = new Container<float?>();
            container.ValueChanged += Container_ValueChanged;
            BlockItemDataOutputDC = new BlockItemDataOutputDC<Container<float?>>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;
        }

        private void Container_ValueChanged(object sender, EventArgs e)
        {
            if (container.Value.HasValue)
                CurrentValue = Math.Round(container.Value.Value, 3).ToString("0.000");
            else
                CurrentValue = string.Empty;
        }

        private void BlockItemDataOutputDC_PullAction(object sender, EventArgs e)
            => BlockItemDataOutputDC.Value = container;

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
