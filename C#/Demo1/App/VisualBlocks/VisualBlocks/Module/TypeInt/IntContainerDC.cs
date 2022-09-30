using Common.SettingBackupAndRestore;
using Common.Types;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeInt
{
    internal class IntContainerDC : BlockItemDC, ICanBackupAndRestore
    {
        public string CurrentValue
        {
            get => currentValue;
            private set => SetField(ref currentValue, value);
        }
        private string currentValue;

        public BlockItemDataOutputDC<Container<int?>> BlockItemDataOutputDC { get; }


        readonly Container<int?> container;


        public IntContainerDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            container = new Container<int?>();
            container.ValueChanged += Container_ValueChanged;
            BlockItemDataOutputDC = new BlockItemDataOutputDC<Container<int?>>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;
        }

        private void Container_ValueChanged(object sender, EventArgs e)
            => CurrentValue = container.Value.ToString();

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
