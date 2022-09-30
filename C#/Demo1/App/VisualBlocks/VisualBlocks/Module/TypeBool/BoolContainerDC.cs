using Common.SettingBackupAndRestore;
using Common.Types;
using System;
using System.Collections.Generic;
using VisualBlocks.Module.Base;


namespace VisualBlocks.Module.TypeBool
{
    internal class BoolContainerDC : BlockItemDC, ICanBackupAndRestore
    {
        public string CurrentValue
        {
            get => currentValue;
            private set => SetField(ref currentValue, value);
        }
        private string currentValue;

        public BlockItemDataOutputDC<Container<bool?>> BlockItemDataOutputDC { get; }


        readonly Container<bool?> container;


        public BoolContainerDC(DependencyParams dependencyParams) : base(dependencyParams)
        {
            container = new Container<bool?>();
            container.ValueChanged += Container_ValueChanged;
            BlockItemDataOutputDC = new BlockItemDataOutputDC<Container<bool?>>();
            BlockItemDataOutputDC.PullAction += BlockItemDataOutputDC_PullAction;
        }

        private void Container_ValueChanged(object sender, EventArgs e)
        {
            if (container.Value.HasValue)
            {
                if (container.Value.Value)
                    CurrentValue = "Igaz";
                else
                    CurrentValue = "Hamis";
            }
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
